using System.Globalization;
using HutchAgent.Config;
using HutchAgent.Constants;
using HutchAgent.Services;
using Microsoft.Extensions.Options;
using Minio.Exceptions;
using File = System.IO.File;
using YamlDotNet.RepresentationModel;

namespace HutchAgent.HostedServices;

[Obsolete]
public class JobPollingHostedService : BackgroundService
{
  private readonly JobActionsQueueOptions _options;
  private readonly WorkflowTriggerOptions _workflowTriggerOptions;
  private readonly ILogger<JobPollingHostedService> _logger;
  private MinioStoreService? _resultsStoreWriter;
  private WorkflowJobService? _WorkflowJobService;
  private CrateService? _crateService;
  private readonly IServiceProvider _serviceProvider;
  private readonly string _workDir;
  private readonly string _statePath = Path.Combine("meta", "execution-state.yaml");

  public JobPollingHostedService(IOptions<JobActionsQueueOptions> options,
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

    // get absolute path to workdir from local config path
    var configYamlDirectory = Path.GetDirectoryName(Path.GetFullPath(_workflowTriggerOptions.LocalConfigPath)) ?? throw new InvalidOperationException();
    _workDir = Path.GetFullPath(Path.Combine(configYamlDirectory, _workDir), configYamlDirectory);
    _logger.LogInformation($"Found working directory {_workDir}");
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
        _resultsStoreWriter = scope.ServiceProvider.GetService<MinioStoreService>() ??
                              throw new InvalidOperationException();
        _WorkflowJobService = scope.ServiceProvider.GetService<WorkflowJobService>() ?? throw new InvalidOperationException();
        _crateService = scope.ServiceProvider.GetService<CrateService>() ??
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
    if (_WorkflowJobService is null) throw new NullReferenceException("_WorkflowJobService instance not available");
    var finishedJobs = await _WorkflowJobService.List();
    foreach (var job in finishedJobs)
    {
      if (job.ExitCode != 0)
      {
        _logger.LogWarning("Job {} did not finish successfully; skipping upload.", job.Id);
        continue;
      }

      var pathToUpload = Path.Combine(
        _workDir,
        job.ExecutorRunId,
        "outputs",
        "execution.crate.zip");

      // Rename the execution crate to match the run ID so it is unique.
      var pathToUploadInfo = new FileInfo(pathToUpload);
      if (!pathToUploadInfo.Exists)
      {
        _logger.LogError("{} does not exist. Cannot upload {} to store.", pathToUpload, pathToUpload);
        continue;
      }

      pathToUploadInfo.MoveTo(
        pathToUploadInfo.FullName.Replace(
          "execution.crate",
          job.ExecutorRunId)
      );

      if (_resultsStoreWriter is null) throw new NullReferenceException("_resultsStoreWriter instance not available");
      if (await _resultsStoreWriter.ResultExists(pathToUploadInfo.Name))
      {
        _logger.LogInformation("{Name} already exists in S3.", pathToUploadInfo.Name);
        continue;
      }

      try
      {
        _logger.LogInformation("Attempting to upload {Name} to S3.", pathToUploadInfo.Name);
        await _resultsStoreWriter.WriteToStore(pathToUploadInfo.FullName);
        _logger.LogInformation("Successfully uploaded {Name} to S3.", pathToUploadInfo.Name);
      }
      catch (BucketNotFoundException)
      {
        _logger.LogCritical("Unable to upload {Name} to S3. The configured bucket does not exist.",
          pathToUploadInfo.Name);
      }
      catch (MinioException)
      {
        _logger.LogError("Unable to upload {Name} to S3. An error occurred with the S3 server.", pathToUploadInfo.Name);
      }
    }
  }

  private async Task MergeResults()
  {
    if (_WorkflowJobService is null) throw new NullReferenceException("_WorkflowJobService instance not available");
    var finishedJobs = await _WorkflowJobService.List();
    foreach (var job in finishedJobs)
    {
      var jobWorkDir = Path.Combine(
        _workDir,
        job.ExecutorRunId);
      var sourceZip = Path.Combine(jobWorkDir, "outputs", $"{job.ExecutorRunId}.zip");
      var pathToMetadata = Path.Combine(job.WorkingDirectory); // directory path to ro-crate-metadata.json
      var mergeDirInfo = new DirectoryInfo(job.WorkingDirectory);
      var mergeDirParent = mergeDirInfo.Parent;
      var mergedZip = Path.Combine(mergeDirParent!.FullName, $"{mergeDirInfo.Name}-merged.zip");
      var pathToContainerImagesDir = Path.Combine(jobWorkDir, "containers");

      if (!File.Exists(sourceZip))
      {
        _logger.LogError("Could not locate {sourceZip}.", sourceZip);
        continue;
      }

      if (_crateService is null) throw new NullReferenceException("_crateService instance not available");
      _crateService.MergeCrates(sourceZip, job.WorkingDirectory);

      // Delete containers directory
      Directory.Delete(pathToContainerImagesDir, recursive: true);
      _crateService.UpdateMetadata(Path.Combine(pathToMetadata, "data"), new()); // job);
      _crateService.ZipCrate(job.WorkingDirectory);
      var jobCrate = _crateService.InitialiseCrate(Path.Combine(pathToMetadata, "data"));
      _crateService.CreateDisclosureCheck(jobCrate);
      jobCrate.Save(Path.Combine(pathToMetadata, "data"));

      if (_resultsStoreWriter is null) throw new NullReferenceException("_resultsStoreWriter instance not available");
      if (await _resultsStoreWriter.ResultExists(mergedZip))
      {
        _logger.LogInformation("Merged Crate {mergedZip} already exists in results store.", mergedZip);
        continue;
      }

      await _resultsStoreWriter.WriteToStore(mergedZip);
      await _WorkflowJobService.Delete(job.Id);
    }
  }

  /// <summary>
  /// Check if WfExS jobs are finished and update the database.
  /// </summary>
  private async Task CheckJobsFinished()
  {
    if (_WorkflowJobService is null) throw new NullReferenceException("_WorkflowJobService instance not available");
    var unfinishedJobs = await _WorkflowJobService.List();
    unfinishedJobs = unfinishedJobs.FindAll(x => x.EndTime is not null);
    foreach (var job in unfinishedJobs)
    {
      // find execution-state.yml for job
      var pathToState = Path.Combine(_workDir, job.ExecutorRunId, _statePath);
      if (!File.Exists(pathToState))
      {
        _logger.LogWarning("Could not find execution status file at '{pathToState}'", pathToState);
        continue;
      }

      var stateYaml = await File.ReadAllTextAsync(pathToState);
      var configYamlStream = new StringReader(stateYaml);
      var yamlStream = new YamlStream();
      yamlStream.Load(configYamlStream);
      var rootNode = yamlStream.Documents[0].RootNode[0];
      // 2. get the exit code
      var exitCode = int.Parse(rootNode["exitVal"].ToString());
      job.ExitCode = exitCode;

      // get start and end times
      DateTime.TryParse(rootNode["started"].ToString(),
        CultureInfo.InvariantCulture,
        DateTimeStyles.AdjustToUniversal,
        out var startTime);
      job.ExecutionStartTime = startTime;
      DateTime.TryParse(rootNode["ended"].ToString(),
        CultureInfo.InvariantCulture,
        DateTimeStyles.AdjustToUniversal,
        out var endTime);
      job.EndTime = endTime;

      // set job to finished
      _logger.LogInformation($"Run {job.ExecutorRunId} finished.");
      // 4. update job in DB
      await _WorkflowJobService.Set(job);

      // 5. set execute action status
      if (_crateService is null) throw new NullReferenceException("_crateService instance not available");
      var jobCrate = _crateService.InitialiseCrate(Path.Combine(job.WorkingDirectory, "data"));
      var executeAction = _crateService.GetExecuteEntity(jobCrate);
      _crateService.UpdateCrateActionStatus(ActionStatus.CompletedActionStatus, executeAction);
      executeAction.SetProperty("endTime",DateTime.Now);
      _logger.LogInformation($"Saving updated RO-Crate metadata to {Path.Combine(job.WorkingDirectory, "data")} .");
      jobCrate.Save(Path.Combine(job.WorkingDirectory, "data"));
    }
  }
}
