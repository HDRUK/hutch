using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.Text.RegularExpressions;
using HutchAgent.Config;
using HutchAgent.Constants;
using HutchAgent.Models;
using HutchAgent.Models.Wfexs;
using HutchAgent.Results;
using Microsoft.Extensions.Options;
using ROCrates;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace HutchAgent.Services;

// TODO this is all pretty wfexs specific;
// maybe in future could be abstracted into a wfexs implementation of a more general interface?
public class WorkflowTriggerService
{
  private readonly WorkflowTriggerOptions _workflowOptions;
  private readonly ILogger<WorkflowTriggerService> _logger;
  private readonly string _activateVenv;
  private const string _bashCmd = "bash";
  private readonly FiveSafesCrateService _crateService;
  private readonly IDeserializer _unyaml;

  public WorkflowTriggerService(
    IOptions<WorkflowTriggerOptions> workflowOptions,
    ILogger<WorkflowTriggerService> logger,
    FiveSafesCrateService crateService)
  {
    _logger = logger;
    _crateService = crateService;
    _workflowOptions = workflowOptions.Value;
    _activateVenv = "source " + _workflowOptions.VirtualEnvironmentPath;
    _unyaml = new DeserializerBuilder()
      .WithNamingConvention(CamelCaseNamingConvention.Instance)
      .IgnoreUnmatchedProperties()
      .Build();
  }

  public string GetExecutorWorkingDirectory()
  {
    // Find the WfExS cache directory path
    var rawLocalConfig = File.ReadAllText(_workflowOptions.LocalConfigPath
                                          ?? throw new InvalidOperationException(
                                            "Workflow Executor Config Path is missing!"));

    var localConfig = _unyaml.Deserialize<WfexsLocalConfig>(rawLocalConfig);

    var relativeWorkDir = localConfig.WorkDir;

    return Path.GetFullPath(
      relativeWorkDir,
      Path.GetDirectoryName(_workflowOptions.LocalConfigPath)
      ?? throw new InvalidOperationException());
  }

  public async Task<WorkflowCompletionResult> HasCompleted(string executorRunId)
  {
    var result = new WorkflowCompletionResult();

    // find execution-state.yml for job
    var pathToState = Path.Combine(
      GetExecutorWorkingDirectory(),
      executorRunId,
      "meta", "execution-state.yaml");
    
    if (!File.Exists(pathToState))
    {
      _logger.LogDebug("Could not find execution status file at '{StatePath}'", pathToState);
      return result;
    }

    result.IsComplete = true;

    var state = _unyaml.Deserialize<WfexsExecutionState>(await File.ReadAllTextAsync(pathToState));

    result.ExitCode = state.ExitCode;
    result.StartTime = state.StartTime;
    result.EndTime = state.EndTime;

    return result;
  }

  public void UnpackOutputs(string executorRunId, string targetPath)
  {
    // Path the to the job outputs
    var executionCratePath = Path.Combine(
      GetExecutorWorkingDirectory(),
      executorRunId,
      "outputs",
      "execution.crate.zip");

    if (!Directory.Exists(targetPath))
      Directory.CreateDirectory(targetPath);
      
    ZipFile.ExtractToDirectory(executionCratePath, targetPath);
    
    
    // Path to workflow containers // TODO this should be INSIDE the unpacked crate!
    var containersPath = Path.Combine(
      "", //_wfexsWorkDir,
      executorRunId,
      "containers");
    
    //if (_workflowOptions.IncludeContainersInOutput) Directory.Delete(containersPath, recursive: true);
  }

  /// <summary>
  /// Execute workflow given a job Id and input crate
  /// </summary>
  /// <param name="job"></param>
  /// <param name="roCrate"></param>
  /// <exception cref="Exception"></exception>
  public async Task TriggerWfexs(WorkflowJob job, ROCrate roCrate)
  {
    //Get execute action and set status to active
    var executeAction = _crateService.GetExecuteEntity(roCrate);
    executeAction.SetProperty("startTime", DateTime.Now);
    _crateService.UpdateCrateActionStatus(ActionStatus.ActiveActionStatus, executeAction);

    // Temporarily - Generate stage file path assuming it comes in input RO-Crate
    var stageFilePath = job.WorkingDirectory.BagItPayloadPath();
    //Get stage file name from RO-Crate
    //Will not be needed once we can generate the stage file
    var stageFileName = _crateService.GetStageFileName(roCrate);
    // Commands to install WfExS and execute a workflow
    // given a path to the local config file and a path to the stage file of a workflow
    var command =
      $"./WfExS-backend.py  -L {_workflowOptions.LocalConfigPath} execute -W {Path.Combine(stageFilePath, stageFileName)}";
    var processStartInfo = new ProcessStartInfo
    {
      RedirectStandardOutput = true,
      RedirectStandardInput = true,
      RedirectStandardError = true,
      UseShellExecute = false,
      CreateNoWindow = true,
      FileName = _bashCmd,
      WorkingDirectory = _workflowOptions.ExecutorPath
    };
    // start process
    var process = Process.Start(processStartInfo);
    if (process == null)
      throw new Exception("Could not start process");
    _logger.LogInformation($"Process started for job: {job.Id}");

    await process.StandardInput.WriteLineAsync(_activateVenv);
    await process.StandardInput.WriteLineAsync(command);
    await process.StandardInput.FlushAsync();
    process.StandardInput.Close();

    // Read the stdout of the WfExS run to get the run ID
    var reader = process.StandardOutput;
    while (!process.HasExited && string.IsNullOrEmpty(job.ExecutorRunId))
    {
      var stdOutLine = await reader.ReadLineAsync();
      if (stdOutLine is null) continue;
      var runName = _findRunName(stdOutLine);
      if (runName is null) continue;
      job.ExecutorRunId = runName;
    }

    // close our connection to the process
    process.Close();
  }

  [Obsolete]
  private string RewritePath(WorkflowJob workflowJob, string line)
  {
    var (linePrefix, relativePath) = line.Split("crate://") switch
    {
      { Length: 2 } a => (a[0], a[1]),
      _ => (null, null)
    };
    if (relativePath is null) return line;

    var absolutePath = Path.Combine(Path.GetFullPath(workflowJob.WorkingDirectory), relativePath);
    var newLine = linePrefix + "file://" + absolutePath;

    _logger.LogInformation($"Writing absolute input path {newLine}");

    return newLine;
  }

  private string? _findRunName(string text)
  {
    var pattern =
      @".*-\sInstance\s([0-9a-fA-F]{8}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{12}).*";
    var regex = new Regex(pattern);

    var match = regex.Match(text);
    if (!match.Success)
    {
      _logger.LogError("Didn't match the pattern!");
      return null;
    }

    // Get the matched UUID pattern
    var uuid = match.Groups[1].Value;
    return Guid.TryParse(uuid, out var validUuid) ? validUuid.ToString() : null;
  }
}
