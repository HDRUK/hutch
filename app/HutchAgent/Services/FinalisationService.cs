using System.Globalization;
using System.IO.Compression;
using HutchAgent.Config;
using HutchAgent.Constants;
using HutchAgent.Models;
using Microsoft.Extensions.Options;
using YamlDotNet.RepresentationModel;

namespace HutchAgent.Services;

public class FinalisationService
{
  private readonly BagItService _bagItService;
  private readonly CrateService _crateService;
  private readonly ILogger<FinalisationService> _logger;
  private readonly IResultsStoreWriter _storeWriter;
  private readonly WorkflowJobService _jobService;
  private readonly PathOptions _pathOptions;
  private readonly IQueueWriter _queueWriter;
  private readonly JobActionsQueueOptions _jobActionsQueue;
  private readonly WorkflowTriggerOptions _workflowOptions;
  private readonly LicenseOptions _licenseOptions;
  private readonly string _wfexsWorkDir;
  private readonly string _statePath = Path.Combine("meta", "execution-state.yaml");

  public FinalisationService(
    BagItService bagItService,
    CrateService crateService,
    ILogger<FinalisationService> logger,
    IResultsStoreWriter storeWriter,
    WorkflowJobService jobService,
    IOptions<PathOptions> pathOptions,
    IOptions<WorkflowTriggerOptions> triggerOptions,
    IQueueWriter queueWriter,
    IOptions<JobActionsQueueOptions> jobActionsQueue,
    IOptions<WorkflowTriggerOptions> workflowOptions)
    IOptions<LicenseOptions> licenseOptions)
  {
    _bagItService = bagItService;
    _crateService = crateService;
    _logger = logger;
    _storeWriter = storeWriter;
    _jobService = jobService;
    _queueWriter = queueWriter;
    _jobActionsQueue = jobActionsQueue.Value;
    _workflowOptions = workflowOptions.Value;
    _licenseOptions = licenseOptions.Value;
    _pathOptions = pathOptions.Value;

    // Find the WfExS cache directory path
    var configYaml = File.ReadAllText(triggerOptions.Value.LocalConfigPath);
    var configYamlStream = new StringReader(configYaml);
    var yamlStream = new YamlStream();
    yamlStream.Load(configYamlStream);
    var rootNode = yamlStream.Documents[0].RootNode;
    _wfexsWorkDir = rootNode["workDir"].ToString();

    // get absolute path to workdir from local config path
    var configYamlDirectory = Path.GetDirectoryName(Path.GetFullPath(triggerOptions.Value.LocalConfigPath)) ??
                              throw new InvalidOperationException();
    _wfexsWorkDir = Path.GetFullPath(Path.Combine(configYamlDirectory, _wfexsWorkDir), configYamlDirectory);
    _logger.LogInformation($"Found working directory {_wfexsWorkDir}");
  }

  /// <summary>
  /// <para>Perform post-execution processes on a workflow run.</para>
  /// These steps are:
  /// <list type="bullet">
  /// <item>Update the state with the start and end times, and the exit code</item>
  /// <item>Merge the results RO-Crate into the input RO-Crate</item>
  /// <item>Update the metadata of the input RO-Crate with the outputs of the workflow run.</item>
  /// <item>Perform disclosure checks on the BagIt.</item>
  /// <item>Re-write the checksums of the BagIt containing the inputs and outputs.</item>
  /// <item>Upload a zipped BagIt to the results store.</item>
  /// <item>Remove the given job from the DB and its associated data from disk.</item>
  /// </list>
  /// </summary>
  /// <param name="jobId"></param>
  public async Task Finalise(string jobId)
  {
    try
    {
      var job = await _jobService.Get(jobId);

      // Finalisation
      await UpdateJob(job);
      MergeCrate(job);
      UpdateMetadata(job);
      DisclosureCheck(job);
      await MakeChecksums(job);

      // Post-finalisation clean-up
      await UploadToStore(job);
      DeleteJobData(job);
      await RemoveJobRecord(job);
    }
    catch (KeyNotFoundException)
    {
      _logger.LogError("Could not find job with ID {}", jobId);
    }
    catch (Exception)
    {
      _logger.LogError("An unexpected error occurred while finalising job {}", jobId);
    }
  }

  /// <summary>
  /// Merge a result crate from a workflow run back into its input crate.
  /// </summary>
  /// <param name="job">The workflow run that needs merging.</param>
  private void MergeCrate(WorkflowJob job)
  {
    // Path the to the job outputs
    var executionCratePath = Path.Combine(
      _wfexsWorkDir,
      job.ExecutorRunId,
      "outputs",
      "execution.crate.zip");

    // Path to workflow containers
    var containersPath = Path.Combine(
      _wfexsWorkDir,
      job.ExecutorRunId,
      "containers");

    if (_workflowOptions.IncludeContainersInOutput) Directory.Delete(containersPath, recursive: true);

    var mergeIntoPath = Path.Combine(
      job.WorkingDirectory.BagItPayloadPath(),
      "outputs");

    _crateService.MergeCrates(executionCratePath, mergeIntoPath);
  }

  /// <summary>
  /// Make the checksums for the BagIt containing the job's data.
  /// </summary>
  /// <param name="job">The job needing checksums.</param>
  private async Task MakeChecksums(WorkflowJob job)
  {
    await _bagItService.WriteManifestSha512(job.WorkingDirectory);
    await _bagItService.WriteTagManifestSha512(job.WorkingDirectory);
  }

  /// <summary>
  /// Update the metadata of the RO-Crate for the given job ID.
  /// </summary>
  /// <param name="job">The job whose metadata needs updating.</param>
  private void UpdateMetadata(WorkflowJob job)
  {
    var cratePath = job.WorkingDirectory.BagItPayloadPath();

    /*
     Update the job metadata to include the results of the workflow run, the license if configured,
     and update the CreateAction based on the on the exit code from the workflow runner.
    */
    try
    {
      _crateService.UpdateMetadata(cratePath, job);
      if (!string.IsNullOrEmpty(_licenseOptions.Uri) && _licenseOptions.Properties is not null &&
          _licenseOptions.Properties.Count > 0)
        _crateService.AddLicense(cratePath);
      var crate = _crateService.InitialiseCrate(cratePath);
      var executeAction = _crateService.GetExecuteEntity(crate);
      _crateService.UpdateCrateActionStatus(
        job.ExitCode == 0 ? ActionStatus.CompletedActionStatus : ActionStatus.FailedActionStatus, executeAction);
    }
    catch (Exception e) when (e is FileNotFoundException or InvalidDataException)
    {
      _logger.LogError("Unable to update the metadata for job: {}", job.Id);
    }
  }

  /// <summary>
  /// Zip a workflow job's BagIt and add it to the configured results store.
  /// </summary>
  /// <param name="job"></param>
  /// <exception cref="DirectoryNotFoundException">Could not locate the parent directory of the job's BagIt.</exception>
  private async Task UploadToStore(WorkflowJob job)
  {
    var bagitDir = new DirectoryInfo(job.WorkingDirectory);
    var bagitParent = Path.Combine(_pathOptions.WorkingDirectoryBase, _pathOptions.Jobs);
    var pathToZip = Path.Combine(
      bagitParent,
      Path.TrimEndingDirectorySeparator(bagitDir.FullName) + ".zip"
    );

    // Zip the BagIt before upload
    ZipFile.CreateFromDirectory(bagitDir.FullName, pathToZip);
    var zipFile = new FileInfo(pathToZip);

    // Make sure results store exists
    if (!await _storeWriter.StoreExists())
    {
      _logger.LogCritical("Could not write {} to the results store. Store not found.", zipFile.Name);
      return;
    }

    // Check if the result already exists
    if (await _storeWriter.ResultExists(zipFile.Name))
    {
      _logger.LogInformation(
        "{} already exists in results store, overwriting.", zipFile.Name);
    }

    // Upload the zipped BagIt
    _logger.LogInformation("Adding {} to results store.", zipFile.Name);
    await _storeWriter.WriteToStore(pathToZip);

    // Delete the zipped BagIt to save disk space
    zipFile.Delete();
  }

  /// <summary>
  /// Update a <see cref="Data.Entities.WorkflowJob"/> in the database to include run start and end times,
  /// and exit code.
  /// </summary>
  /// <param name="job">The job entity to update.</param>
  private async Task UpdateJob(WorkflowJob job)
  {
    // find execution-state.yml for job
    var pathToState = Path.Combine(_wfexsWorkDir, job.ExecutorRunId, _statePath);
    if (!File.Exists(pathToState))
    {
      _logger.LogWarning("Could not find execution status file at '{}'", pathToState);
      var message = new JobQueueMessage { ActionType = JobActionTypes.Execute, JobId = job.Id };
      _queueWriter.SendMessage(_jobActionsQueue.QueueName, message);
      return;
    }

    var stateYaml = await File.ReadAllTextAsync(pathToState);
    var configYamlStream = new StringReader(stateYaml);
    var yamlStream = new YamlStream();
    yamlStream.Load(configYamlStream);
    var rootNode = yamlStream.Documents[0].RootNode[0];
    // get the exit code
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

    // update job in DB
    await _jobService.Set(job);
  }

  /// <summary>
  /// Perform disclosure checks for a given job.
  /// </summary>
  /// <param name="job">The job needing disclosure checks.</param>
  private void DisclosureCheck(WorkflowJob job)
  {
    var crate = _crateService.InitialiseCrate(job.WorkingDirectory.BagItPayloadPath());
    _crateService.CreateDisclosureCheck(crate);
    crate.Save(job.WorkingDirectory.BagItPayloadPath());
  }

  /// <summary>
  /// Removes a job from the database.
  /// </summary>
  /// <param name="job">The job to remove.</param>
  private async Task RemoveJobRecord(WorkflowJob job)
  {
    await _jobService.Delete(job.Id);
  }

  /// <summary>
  /// Delete the data relating to the given job from disk.
  /// </summary>
  /// <param name="job">The job whose data need deleting.</param>
  private void DeleteJobData(WorkflowJob job)
  {
    if (Directory.Exists(job.WorkingDirectory)) Directory.Delete(job.WorkingDirectory, recursive: true);
  }
}
