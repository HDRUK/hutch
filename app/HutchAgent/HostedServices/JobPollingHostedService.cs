using HutchAgent.Config;
using HutchAgent.Services;
using Microsoft.Extensions.Options;
using Minio.Exceptions;
using YamlDotNet.RepresentationModel;

namespace HutchAgent.HostedServices;

public class JobPollingHostedService : BackgroundService
{
  private readonly JobPollingOptions _options;
  private readonly WorkflowTriggerOptions _workflowTriggerOptions;
  private readonly ILogger<JobPollingHostedService> _logger;
  private IResultsStoreWriter? _resultsStoreWriter;
  private WfexsJobService? _wfexsJobService;
  private CrateMergerService? _crateMergerService;
  private readonly IServiceProvider _serviceProvider;
  private readonly string _workDir;
  private readonly string _statePath = Path.Combine("meta", "execution-state.yaml");

  public JobPollingHostedService(IOptions<JobPollingOptions> options,
    IOptions<WorkflowTriggerOptions> workflowTriggerOptions, ILogger<JobPollingHostedService> logger,
    IServiceProvider serviceProvider)
  {
    _options = options.Value;
    _logger = logger;
    _serviceProvider = serviceProvider;
    _workflowTriggerOptions = workflowTriggerOptions.Value;

    // Find the WfExS cache directory path
    var configYaml = File.ReadAllText(_workflowTriggerOptions.LocalConfigPath);
    var configYamlStream = new StringReader(configYaml);
    var yamlStream = new YamlStream();
    yamlStream.Load(configYamlStream);
    var rootNode = yamlStream.Documents[0].RootNode;
    _workDir = rootNode["workDir"].ToString();
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
    if (_wfexsJobService is null) throw new NullReferenceException("_wfexsJobService instance not available");
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

      if (_resultsStoreWriter is null) throw new NullReferenceException("_resultsStoreWriter instance not available");
      if (await _resultsStoreWriter.ResultExists(pathToUploadInfo.Name))
      {
        _logger.LogInformation($"{pathToUploadInfo.Name} already exists in S3.");
        continue;
      }

      try
      {
        _logger.LogInformation($"Attempting to upload {pathToUploadInfo.Name} to S3.");
        await _resultsStoreWriter.WriteToStore(pathToUploadInfo.FullName);
        _logger.LogInformation($"Successfully uploaded {pathToUploadInfo.Name} to S3.");
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
    if (_wfexsJobService is null) throw new NullReferenceException("_wfexsJobService instance not available");
    var finishedJobs = await _wfexsJobService.ListFinishedJobs();
    foreach (var job in finishedJobs)
    {
      var jobWorkDir = Path.Combine(
        _workflowTriggerOptions.ExecutorPath,
        "wfexs-backend-test_WorkDir",
        job.WfexsRunId);
      var sourceZip = Path.Combine(jobWorkDir, "outputs", $"{job.WfexsRunId}.zip");
      var pathToMetadata = Path.Combine(job.UnpackedPath); // directory path to ro-crate-metadata.json
      var mergeDirInfo = new DirectoryInfo(job.UnpackedPath);
      var mergeDirParent = mergeDirInfo.Parent;
      var mergedZip = Path.Combine(mergeDirParent!.FullName, $"{mergeDirInfo.Name}-merged.zip");
      var pathToContainerImagesDir = Path.Combine(jobWorkDir, "containers");

      if (!File.Exists(sourceZip))
      {
        _logger.LogError($"Could not locate {sourceZip}.");
        continue;
      }

      if (_crateMergerService is null) throw new NullReferenceException("_crateMergerService instance not available");
      _crateMergerService.MergeCrates(sourceZip, job.UnpackedPath);
      _crateMergerService.DeleteContainerImages(pathToContainerImagesDir);
      _crateMergerService.UpdateMetadata(pathToMetadata);
      _crateMergerService.ZipCrate(job.UnpackedPath);

      if (_resultsStoreWriter is null) throw new NullReferenceException("_resultsStoreWriter instance not available");
      if (await _resultsStoreWriter.ResultExists(mergedZip))
      {
        _logger.LogInformation($"Merged Crate {mergedZip} already exists in results store.");
        continue;
      }

      await _resultsStoreWriter.WriteToStore(mergedZip);
      await _wfexsJobService.Delete(job.Id);
    }
  }

  /// <summary>
  /// Check if WfExS jobs are finished and update the database.
  /// </summary>
  private async Task CheckJobsFinished()
  {
    if (_wfexsJobService is null) throw new NullReferenceException("_wfexsJobService instance not available");
    var unfinishedJobs = await _wfexsJobService.List();
    unfinishedJobs = unfinishedJobs.FindAll(x => !x.RunFinished);

    foreach (var job in unfinishedJobs)
    {
      // 1. find execution-state.yml for job
      var pathToState = Path.Combine(_workDir, _statePath);
      if (!File.Exists(pathToState))
      {
        _logger.LogWarning("Could not find execution status file at '{pathToState}'", pathToState);
        continue;
      }

      var stateYaml = await File.ReadAllTextAsync(pathToState);
      var configYamlStream = new StringReader(stateYaml);
      var yamlStream = new YamlStream();
      yamlStream.Load(configYamlStream);
      var rootNode = yamlStream.Documents[0].RootNode;

      // 2. get the exit code
      var exitCode = int.Parse(rootNode["exitVal"].ToString());
      job.ExitCode = exitCode;
      // record start and finish times?

      // 3. set job to finished
      job.RunFinished = true;

      // 4. update job in DB
      await _wfexsJobService.Set(job);
    }
  }
}
