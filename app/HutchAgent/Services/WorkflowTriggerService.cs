using System.Diagnostics;
using System.Text.RegularExpressions;
using HutchAgent.Config;
using HutchAgent.Constants;
using HutchAgent.Models;
using Microsoft.Extensions.Options;
using ROCrates;

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
  /// Execute Wfexs given a job Id and input crate
  /// </summary>
  /// <param name="job"></param>
  /// <param name="roCrate"></param>
  /// <exception cref="Exception"></exception>
  public async Task TriggerWfexs(Data.Entities.WorkflowJob job,ROCrate roCrate)
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
