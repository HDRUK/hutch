using System.Globalization;
using System.IO.Compression;
using HutchAgent.Config;
using HutchAgent.Constants;
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
  private readonly string _wfexsWorkDir;
  private readonly string _statePath = Path.Combine("meta", "execution-state.yaml");

  public FinalisationService(
    BagItService bagItService,
    CrateService crateService,
    ILogger<FinalisationService> logger,
    IResultsStoreWriter storeWriter,
    WorkflowJobService jobService,
    IOptions<PathOptions> pathOptions,
    IOptions<WorkflowTriggerOptions> triggerOptions)
  {
    _bagItService = bagItService;
    _crateService = crateService;
    _logger = logger;
    _storeWriter = storeWriter;
    _jobService = jobService;
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
  /// <item>Re-write the checksums of the BagIt containing the inputs and outputs.</item>
  /// <item>Upload a zipped BagIt to the results store.</item>
  /// </list>
  /// </summary>
  /// <param name="jobId"></param>
  public async Task Finalise(string jobId)
  {
    await UpdateJob(jobId);
    await MergeCrate(jobId);
    await UpdateMetadata(jobId);
    await MakeChecksums(jobId);
    await UploadToStore(jobId);
  }

  /// <summary>
  /// Merge a result crate from a workflow run back into its input crate.
  /// </summary>
  /// <param name="jobId">The ID of the workflow run that needs merging.</param>
  private async Task MergeCrate(string jobId)
  {
    var job = await _jobService.Get(jobId);

    // Path the to the job outputs
    var executionCratePath = Path.Combine(
      _wfexsWorkDir,
      job.ExecutorRunId,
      "outputs",
      "execution.crate.zip");

    var mergeIntoPath = Path.Combine(
      job.WorkingDirectory.BagItPayloadPath(),
      "outputs");

    _crateService.MergeCrates(executionCratePath, mergeIntoPath);
  }

  /// <summary>
  /// Make the checksums for the BagIt containing the job's data.
  /// </summary>
  /// <param name="jobId">The ID of the job needing checksums.</param>
  private async Task MakeChecksums(string jobId)
  {
    var job = await _jobService.Get(jobId);
    await _bagItService.WriteManifestSha512(job.WorkingDirectory);
    await _bagItService.WriteTagManifestSha512(job.WorkingDirectory);
  }

  /// <summary>
  /// Update the metadata of the RO-Crate for the given job ID.
  /// </summary>
  /// <param name="jobId">The ID of the job that needs updating.</param>
  private async Task UpdateMetadata(string jobId)
  {
    var job = await _jobService.Get(jobId);

    var metadataPath = job.WorkingDirectory.BagItPayloadPath();

    /*
     Update the job metadata to include the results of the workflow run and update the CreateAction
     based on the on the exit code from the workflow runner.
    */
    try
    {
      _crateService.UpdateMetadata(metadataPath, job);
      var crate = _crateService.InitialiseCrate(metadataPath);
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
  /// <param name="jobId"></param>
  /// <exception cref="DirectoryNotFoundException">Could not locate the parent directory of the job's BagIt.</exception>
  private async Task UploadToStore(string jobId)
  {
    var job = await _jobService.Get(jobId);
    var bagitDir = new DirectoryInfo(job.WorkingDirectory);
    var bagitParent = bagitDir.Parent ??
                      throw new DirectoryNotFoundException($"Cannot find parent directory of {bagitDir.FullName}");
    var pathToZip = Path.Combine(
      bagitParent.FullName,
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
  /// <param name="jobId">The ID of the job entity to update.</param>
  private async Task UpdateJob(string jobId)
  {
    var job = await _jobService.Get(jobId);

    // find execution-state.yml for job
    var pathToState = Path.Combine(_wfexsWorkDir, job.ExecutorRunId, _statePath);
    if (!File.Exists(pathToState))
    {
      _logger.LogWarning("Could not find execution status file at '{}'", pathToState);
      // Todo: re-queue the because the job hasn't finished yet??
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
}
