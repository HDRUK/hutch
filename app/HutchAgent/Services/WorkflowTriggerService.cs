using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using HutchAgent.Config;
using HutchAgent.Constants;
using HutchAgent.Models;
using Microsoft.Extensions.Options;
using ROCrates;
using ROCrates.Models;
using YamlDotNet.Serialization;
namespace HutchAgent.Services;

public class WorkflowTriggerService
{
  private readonly WorkflowTriggerOptions _workflowOptions;
  private readonly ILogger<WorkflowTriggerService> _logger;
  private readonly string _activateVenv;
  private const string _bashCmd = "bash";
  private readonly CrateService _crateService;

  public WorkflowTriggerService(
    IOptions<WorkflowTriggerOptions> workflowOptions,
    ILogger<WorkflowTriggerService> logger,
    CrateService crateService)
  {
    _logger = logger;
    _crateService = crateService;
    _workflowOptions = workflowOptions.Value;
    _activateVenv = "source " + _workflowOptions.VirtualEnvironmentPath;
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
    var stageFilePath = job.WorkingDirectory.JobBagItRoot().BagItPayloadPath();
    // Create Stage file
    WriteStageFile(job, roCrate);
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

  private void WriteStageFile(WorkflowJob workflowJob, ROCrate roCrate)
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
    // var workflowEntity = workflowCrate.Entities[workflow.Id];

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
    var inputParameter = new List<object>();
    using (StreamWriter outputStageFile =
           new StreamWriter(Path.Combine(workflowJob.WorkingDirectory.JobBagItRoot().BagItPayloadPath(),
             "hutch_cwl.wfex.stage")))
    {
      var fileInput = new Dictionary<string, string>();

      foreach (var queryObject in queryObjects)
      {
        var id = queryObject?["@id"] ?? throw new InvalidOperationException($"No key @id found in {queryObject}");
        var objectEntity = roCrate.Entities[id.ToString()] ??
                           throw new NullReferenceException($"No Entity with id {id} found in RO-Crate");
        var exampleOfWork = objectEntity.GetProperty<Part>("exampleOfWork")!.Id;
        var parameter = roCrate.Entities[exampleOfWork];
        var name = parameter.Properties["name"]!.ToString();
        var type = objectEntity.Properties["@type"]!.ToString();

        if (type is "File")
        {
          var absolutePath = Path.Combine(
            Path.GetFullPath(workflowJob.WorkingDirectory.JobBagItRoot().BagItPayloadPath()),
            objectEntity.Id);
          var filePath = "file://" + absolutePath;
          var values = new Dictionary<string, string>()
          {
            { "c-l-a-s-s", "File" },
            { "url", filePath }
          };
          var json = JsonSerializer.Serialize(values);
          fileInput.Add(name, json);
        }

        else
        {
          var value = objectEntity.Properties["value"] ??
                      throw new NullReferenceException("Could not get value for given input parameter.");
          fileInput.Add(name, value.ToString());
        }
      }

      stageFile.Params = fileInput;
      var serializer = new SerializerBuilder()
        .Build();
      var yaml = serializer.Serialize(stageFile);

      outputStageFile.WriteLine(yaml);
    }
  }

  private async void InitialiseRepo(string repoPath)
  {
    var gitCommands = new List<string>()
    {
      "git init",
      "git add .",
      @"git commit -m ""Initialise repo"" ",
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
    var absolutePath = "git+" + "file://" + Path.GetFullPath(pathToGitRepo) + ".git" + "#subdirectory=" +
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
