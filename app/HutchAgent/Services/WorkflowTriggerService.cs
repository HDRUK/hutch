using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
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
using ROCrates.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using File = System.IO.File;

namespace HutchAgent.Services;

// TODO this is all pretty wfexs specific;
// maybe in future could be abstracted into a wfexs implementation of a more general interface?
public class WorkflowTriggerService
{
  private readonly WorkflowTriggerOptions _workflowOptions;
  private readonly ILogger<WorkflowTriggerService> _logger;
  private readonly string _activateVenv;
  private const string _bashCmd = "bash";
  private readonly CrateService _crateService;
  private readonly IDeserializer _unyaml;

  public WorkflowTriggerService(
    IOptions<WorkflowTriggerOptions> workflowOptions,
    ILogger<WorkflowTriggerService> logger,
    CrateService crateService)
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
    
    // Create stage file and save file path
    var stageFilePath = WriteStageFile(job, roCrate);
    // Commands to install WfExS and execute a workflow
    // given a path to the local config file and a path to the stage file of a workflow
    var command =
      $"./WfExS-backend.py  -L {_workflowOptions.LocalConfigPath} execute -W {stageFilePath}";
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

  private string WriteStageFile(WorkflowJob workflowJob, ROCrate roCrate)
  {
    //Get execution details
    var mentions = roCrate.RootDataset.GetProperty<JsonArray>("mentions") ??
                   throw new NullReferenceException("No mentions found in RO-Crate RootDataset Properties");
    var downloadAction = mentions.Where(mention =>
                             mention != null &&
                             roCrate.Entities[mention["@id"]!.ToString()].Properties["@type"]?.ToString() ==
                             "DownloadAction")
                           .Select(mention =>
                             roCrate.Entities[mention!["@id"]!.ToString()].GetProperty<JsonNode>("result")) ??
                         throw new NullReferenceException("No download action found in the RO-Crate");

    var cratePath = Path.Combine(workflowJob.WorkingDirectory.JobBagItRoot().BagItPayloadPath(),
      downloadAction.First()!["@id"]!.ToString());
    InitialiseRepo(cratePath);

    var workflowCrate = _crateService.InitialiseCrate(cratePath);
    var workflow = workflowCrate.RootDataset.GetProperty<Part>("mainEntity") ??
                   throw new NullReferenceException("Cannot find main entity in RootDataset");

    var gitUrl = CreateGitUrl(cratePath, workflow!.Id);

    // Create stage file object
    var stageFile = new WorkflowStageFile()
    {
      WorkflowId = gitUrl,
      Nickname = "HutchAgent" + workflowJob.Id,
      Params = new object()
    };
    // Get inputs from execute entity
    var executeEntity = _crateService.GetExecuteEntity(roCrate);
    var queryObjects = executeEntity.GetProperty<JsonArray>("object")!.ToList();

    var parameters = new Dictionary<string, object>();

    foreach (var queryObject in queryObjects)
    {
      var id = queryObject?["@id"] ?? throw new InvalidOperationException($"No key @id found in {queryObject}");
      var objectEntity = roCrate.Entities[id.ToString()] ??
                         throw new NullReferenceException($"No Entity with id {id} found in RO-Crate");
      var exampleOfWork = objectEntity.GetProperty<Part>("exampleOfWork") ??
                          throw new NullReferenceException($"No exampleOfWork property found in {objectEntity.Id}");
      var parameter = roCrate.Entities[exampleOfWork.Id];
      var name = parameter.Properties["name"] ??
                 throw new NullReferenceException($"No name property found for {parameter.Id}");
      var type = objectEntity.Properties["@type"] ??
                 throw new NullReferenceException($"No type property found for {objectEntity.Id}");

      if (type.ToString() is "File")
      {
        // get absolute path to input
        var absolutePath = Path.Combine(
          Path.GetFullPath(workflowJob.WorkingDirectory.JobBagItRoot().BagItPayloadPath()),
          objectEntity.Id);
        var filePath = "file://" + absolutePath;

        var values = new Dictionary<string, string>()
        {
          ["c-l-a-s-s"] = type.ToString(),
          ["url"] = filePath
        };
        parameters[name.ToString()] = values;
      }
      else
      {
        var value = objectEntity.Properties["value"] ??
                    throw new NullReferenceException("Could not get value for given input parameter.");
        parameters[name.ToString()] = value.ToString();
      }
    }

    // Set input params
    stageFile.Params = parameters;

    // Get outputs from workflow crate
    var workflowMainEntity = workflowCrate.Entities[workflow.Id];
    var outputs = workflowMainEntity.Properties["output"] ??
                  throw new InvalidOperationException("No property 'output' found in Workflow RO-Crate");

    var outputId = outputs["@id"] ?? throw new NullReferenceException("Id not found for output object");
    var outputEntity = workflowCrate.Entities[outputId.ToString()];
    var outputName = outputEntity.Properties["name"] ??
                     throw new InvalidOperationException($"No property 'name' found for output {outputId}");
    var outputParam = new Dictionary<string, object>()
    {
      [outputName.ToString()] = new Dictionary<string, string>()
      {
        ["c-l-a-s-s"] = "File"
      }
    };
    // set outputs in stage file
    stageFile.Outputs = outputParam;
    var serializer = new SerializerBuilder()
      .Build();
    var yaml = serializer.Serialize(stageFile);
    var stageFilePath = Path.Combine(workflowJob.WorkingDirectory.JobBagItRoot().BagItPayloadPath(),
      "hutch_cwl.wfex.stage");
    using (StreamWriter outputStageFile =
           new StreamWriter(stageFilePath))
    {
      outputStageFile.WriteLineAsync(yaml);
    }

    var absoluteStageFilePath = Path.Combine(
      Path.GetFullPath(stageFilePath));

    return absoluteStageFilePath;
  }

  private async void InitialiseRepo(string repoPath)
  {
    var gitCommands = new List<string>()
    {
      "git -c init.defaultBranch=main init",
      "git add .",
      "git -c user.name='Hutch' -c user.email='hutch@example.com' commit -m 'Initialise repo' ",
    };
    ProcessStartInfo gitProcessStartInfo = new ProcessStartInfo()
    {
      RedirectStandardOutput = true,
      RedirectStandardInput = true,
      RedirectStandardError = true,
      UseShellExecute = false,
      CreateNoWindow = true,
      FileName = _bashCmd,
      WorkingDirectory = repoPath
    };

    var gitProcess = Process.Start(gitProcessStartInfo);
    if (gitProcess == null)
      throw new Exception("Could not start process");

    await using var streamWriter = gitProcess.StandardInput;
    if (streamWriter.BaseStream.CanWrite)
    {
      foreach (var command in gitCommands)
      {
        await streamWriter.WriteLineAsync(command);
      }

      await streamWriter.FlushAsync();
      streamWriter.Close();
    }

    gitProcess.Close();
  }

  private string CreateGitUrl(string pathToGitRepo, string pathToWorkflow)
  {
    var absolutePath ="file://" + Path.GetFullPath(pathToGitRepo) + "#subdirectory=" +
                       pathToWorkflow;

    return absolutePath;
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
