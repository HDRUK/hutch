using System.Diagnostics;
using System.Text.RegularExpressions;
using HutchAgent.Models;
using Microsoft.Extensions.Options;

namespace HutchAgent.Services;

public class WorkflowTriggerService : BackgroundService
{
  private readonly WorkflowTriggerOptions _workflowOptions;
  private readonly ILogger<WorkflowTriggerService> _logger;
  private readonly string _activateVenv;
  private const string _bashCmd = "bash";
  private string? _workDirName = null;

  public WorkflowTriggerService(IOptions<WorkflowTriggerOptions> workflowOptions,
    ILogger<WorkflowTriggerService> logger)
  {
    _logger = logger;
    _workflowOptions = workflowOptions.Value;
    _activateVenv = "source " + _workflowOptions.VirtualEnvironmentPath;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    _logger.LogInformation(
      "Executing Workflow with WfExS started.");

    await TriggerWfexs();

    if (_workDirName is null)
    {
      _logger.LogError("Unable to determine Run ID.");
      return;
    }

    await _createProvCrate(_workDirName);
  }

  /// <summary>
  /// Install and run WfExS given 
  /// </summary>
  /// <exception cref="Exception"></exception>
  public async Task TriggerWfexs()
  {
    // Commands to install WfExS and execute a workflow
    // given a path to the local config file and a path to the stage file of a workflow
    var commands = new List<string>()
    {
      $"./WfExS-backend.py  -L {_workflowOptions.LocalConfigPath} execute -W {_workflowOptions.StageFilePath}"
    };

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

    using var streamWriter = process.StandardInput;
    if (streamWriter.BaseStream.CanWrite)
    {
      // activate python virtual environment
      await streamWriter.WriteLineAsync(_activateVenv);
      foreach (var command in commands)
      {
        await streamWriter.WriteLineAsync(command);
      }

      await streamWriter.FlushAsync();
      streamWriter.Close();
    }

    StreamReader reader = process.StandardOutput;
    while (!process.HasExited)
    {
      var stdOutLine = await reader.ReadLineAsync();
      if (stdOutLine is null) continue;
      var runName = _findRunName(stdOutLine);
      if (runName is not null) _workDirName = runName;
    }

    // end the process
    process.Close();
  }

  /// <summary>
  /// Command WfExS to build the RO-Crate of the workflow.
  /// </summary>
  /// <param name="runId">The UUID of the run for which to output the RO-Crate.</param>
  /// <exception cref="Exception"></exception>
  private async Task _createProvCrate(string runId)
  {
    var command = $@"./WfExS_backend.py \
  -L <path_to_wfexs_config.yaml> \
  staged-workdir create-prov-crate {runId} ROCrate.zip \
  --full";

    var processStartInfo = new ProcessStartInfo
    {
      RedirectStandardOutput = false,
      RedirectStandardInput = false,
      RedirectStandardError = false,
      UseShellExecute = false,
      CreateNoWindow = true,
      FileName = _bashCmd,
      WorkingDirectory = _workflowOptions.ExecutorPath
    };

    // start process
    var process = Process.Start(processStartInfo);
    if (process == null)
      throw new Exception("Could not start process");

    using var streamWriter = process.StandardInput;
    if (streamWriter.BaseStream.CanWrite)
    {
      // activate python virtual environment
      await streamWriter.WriteLineAsync(_activateVenv);
      // execute command to build RO-Crate
      await streamWriter.WriteLineAsync(command);

      await streamWriter.FlushAsync();
      streamWriter.Close();
    }

    // Wait for the process to exit
    while (!process.HasExited)
    {
      await Task.Delay(TimeSpan.FromSeconds(1));
    }

    // end the process
    process.Close();
  }

  public override async Task StopAsync(CancellationToken stoppingToken)
  {
    _logger.LogInformation(
      "Hosted Service is stopping.");

    await base.StopAsync(stoppingToken);
  }

  private string? _findRunName(string text)
  {
    var pattern =
      "-\\sInstance\\s([0-9a-fA-F]{8}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{12})";
    var regex = new Regex(pattern);

    var match = regex.Match(text);
    if (!match.Success) return null;
    // Get the matched UUID pattern
    var uuid = match.Captures.GetEnumerator().Current.ToString();
    return Guid.TryParse(uuid, out var validUuid) ? validUuid.ToString() : null;
  }
}
