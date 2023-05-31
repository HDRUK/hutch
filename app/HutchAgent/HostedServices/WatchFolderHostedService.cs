using System.Diagnostics;
using HutchAgent.Config;
using HutchAgent.Services;
using Microsoft.Extensions.Options;
using Minio.Exceptions;

namespace HutchAgent.HostedServices;

public class WatchFolderHostedService : BackgroundService
{
  private readonly WatchFolderOptions _options;
  private readonly ILogger<WatchFolderHostedService> _logger;
  private IResultsStoreWriter? _resultsStoreWriter;
  private WfexsJobService? _wfexsJobService;
  private CrateMergerService? _crateMergerService;
  private readonly IServiceProvider _serviceProvider;

  public WatchFolderHostedService(IOptions<WatchFolderOptions> options, ILogger<WatchFolderHostedService> logger,
    IServiceProvider serviceProvider)
  {
    _options = options.Value;
    _logger = logger;
    _serviceProvider = serviceProvider;
  }

  /// <summary>
  /// Watch a folder for results of WfExS runs and upload to an S3 bucket.
  /// </summary>
  /// <param name="stoppingToken"></param>
  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    _logger.LogInformation($"Starting to watch folder {_options.Path}");

    while (!stoppingToken.IsCancellationRequested)
    {
      using (var scope = _serviceProvider.CreateScope())
      {
        _resultsStoreWriter = scope.ServiceProvider.GetService<IResultsStoreWriter>() ??
                              throw new InvalidOperationException();
        _wfexsJobService = scope.ServiceProvider.GetService<WfexsJobService>() ?? throw new InvalidOperationException();
        _crateMergerService = scope.ServiceProvider.GetService<CrateMergerService>() ??
                              throw new InvalidOperationException();
        await CheckJobsFinished();
        WatchFolder();
        MergeResults();
      }

      await Task.Delay(TimeSpan.FromSeconds(_options.PollingIntervalSeconds), stoppingToken);
    }
  }

  /// <summary>
  /// Stop watching the results folder for WfExS execution results.
  /// </summary>
  /// <param name="stoppingToken"></param>
  public override async Task StopAsync(CancellationToken stoppingToken)
  {
    _logger.LogInformation($"Stopping watching folder {_options.Path}");

    await base.StopAsync(stoppingToken);
  }

  private async void WatchFolder()
  {
    var finishedJobs = await _wfexsJobService.ListFinishedJobs();
    foreach (var job in finishedJobs)
    {
      var pathToUpload = Path.Combine(_options.Path, $"{job.WfexsRunId}.zip");

      if (await _resultsStoreWriter.ResultExists(Path.GetFileName(pathToUpload)))
      {
        _logger.LogInformation($"{Path.GetFileName(pathToUpload)} already exists in S3.");
        continue;
      }

      try
      {
        _logger.LogInformation($"Attempting to upload {pathToUpload} to S3.");
        await _resultsStoreWriter.WriteToStore(pathToUpload);
        _logger.LogInformation($"Successfully uploaded {pathToUpload}.zip to S3.");
      }
      catch (BucketNotFoundException)
      {
        _logger.LogCritical($"Unable to upload {pathToUpload} to S3. The configured bucket does not exist.");
      }
      catch (MinioException)
      {
        _logger.LogError($"Unable to upload {pathToUpload} to S3. An error occurred with the S3 server.");
      }
    }
  }

  private async void MergeResults()
  {
    var finishedJobs = await _wfexsJobService.ListFinishedJobs();
    foreach (var job in finishedJobs)
    {
      var sourceZip = Path.Combine(_options.Path, $"{job.WfexsRunId}.zip");
      var pathToMetadata = Path.Combine(job.UnpackedPath, "ro-crate-metadata.json");
      var mergeDirInfo = new DirectoryInfo(job.UnpackedPath);
      var mergeDirParent = mergeDirInfo.Parent;
      var mergedZip = Path.Combine(mergeDirParent!.ToString(), $"{mergeDirInfo.Name}-merged.zip");

      if (!File.Exists(sourceZip))
      {
        _logger.LogError($"Could not locate {sourceZip}.");
        continue;
      }

      _crateMergerService.MergeCrates(sourceZip, job.UnpackedPath);
      _crateMergerService.UpdateMetadata(pathToMetadata, sourceZip);
      _crateMergerService.ZipCrate(job.UnpackedPath);

      if (!await _resultsStoreWriter.ResultExists(Path.Combine(mergeDirParent.ToString(), mergedZip)))
      {
        _logger.LogError($"Could not locate merged RO-Crate {mergedZip}.");
        continue;
      }

      await _resultsStoreWriter.WriteToStore(Path.Combine(mergeDirParent.ToString(), mergedZip));
    }
  }

  /// <summary>
  /// Check if WfExS jobs are finished and update the database.
  /// </summary>
  private async Task CheckJobsFinished()
  {
    var unfinishedJobs = await _wfexsJobService.List();
    unfinishedJobs = unfinishedJobs.FindAll(x => !x.RunFinished);

    foreach (var job in unfinishedJobs)
    {
      try
      {
        Process.GetProcessById(job.Pid);
      }
      catch (ArgumentException)
      {
        job.RunFinished = true;
        _wfexsJobService.Set(job);
      }
    }
  }
}
