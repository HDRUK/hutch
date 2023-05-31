using System.Diagnostics;
using HutchAgent.Config;
using HutchAgent.Services;
using Microsoft.Extensions.Options;
using Minio.Exceptions;

namespace HutchAgent.HostedServices;

public class WatchFolderHostedService : BackgroundService
{
  private readonly JobPollingOptions _options;
  private readonly WorkflowTriggerOptions _workflowTriggerOptions;
  private readonly ILogger<WatchFolderHostedService> _logger;
  private IResultsStoreWriter? _resultsStoreWriter;
  private WfexsJobService? _wfexsJobService;
  private CrateMergerService? _crateMergerService;
  private readonly IServiceProvider _serviceProvider;

  public WatchFolderHostedService(IOptions<JobPollingOptions> options,
    IOptions<WorkflowTriggerOptions> workflowTriggerOptions, ILogger<WatchFolderHostedService> logger,
    IServiceProvider serviceProvider)
  {
    _options = options.Value;
    _logger = logger;
    _serviceProvider = serviceProvider;
    _workflowTriggerOptions = workflowTriggerOptions.Value;
  }

  /// <summary>
  /// Watch a folder for results of WfExS runs and upload to an S3 bucket.
  /// </summary>
  /// <param name="stoppingToken"></param>
  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    _logger.LogInformation("Polling WfExS Jobs.");

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
        await UploadResults();
        await MergeResults();
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
    _logger.LogInformation("Stopping polling WfExS jobs.");

    await base.StopAsync(stoppingToken);
  }

  private async Task UploadResults()
  {
    var finishedJobs = await _wfexsJobService.ListFinishedJobs();
    foreach (var job in finishedJobs)
    {
      var pathToUpload = Path.Combine(
        _workflowTriggerOptions.ExecutorPath,
        "wfexs-backend-test_WorkDir",
        job.WfexsRunId,
        "outputs",
        "execution.crate.zip");

      // Rename the execution crate to match the run ID so it is unique.
      var pathToUploadInfo = new FileInfo(pathToUpload);
      pathToUploadInfo.MoveTo(
        pathToUploadInfo.FullName.Replace(
          "execution.crate",
          job.WfexsRunId)
      );

      if (await _resultsStoreWriter.ResultExists(pathToUploadInfo.Name))
      {
        _logger.LogInformation($"{pathToUploadInfo.Name} already exists in S3.");
        continue;
      }

      try
      {
        _logger.LogInformation($"Attempting to upload {pathToUploadInfo.Name} to S3.");
        await _resultsStoreWriter.WriteToStore(pathToUploadInfo.Name);
        _logger.LogInformation($"Successfully uploaded {pathToUploadInfo.Name}.zip to S3.");
      }
      catch (BucketNotFoundException)
      {
        _logger.LogCritical($"Unable to upload {pathToUploadInfo.Name} to S3. The configured bucket does not exist.");
      }
      catch (MinioException)
      {
        _logger.LogError($"Unable to upload {pathToUploadInfo.Name} to S3. An error occurred with the S3 server.");
      }
    }
  }

  private async Task MergeResults()
  {
    var finishedJobs = await _wfexsJobService.ListFinishedJobs();
    foreach (var job in finishedJobs)
    {
      var sourceZip = Path.Combine(
        _workflowTriggerOptions.ExecutorPath,
        "wfexs-backend-test_WorkDir",
        job.WfexsRunId,
        "outputs",
        $"{job.WfexsRunId}.zip");
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
        await _wfexsJobService.Set(job);
      }
    }
  }
}
