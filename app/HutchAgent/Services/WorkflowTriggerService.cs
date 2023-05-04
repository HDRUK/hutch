using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using HutchAgent.Models;
using Microsoft.Extensions.Options;

namespace HutchAgent.Services;

public class WorkflowTriggerService : BackgroundService
{
  private readonly WorkflowTriggerOptions _workflowOptions;
  private readonly ILogger<WorkflowTriggerService> _logger;

  public WorkflowTriggerService(IOptions<WorkflowTriggerOptions> workflowOptions,
    ILogger<WorkflowTriggerService> logger)
  {
    _logger = logger;
    _workflowOptions = workflowOptions.Value;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    _logger.LogInformation(
      "Executing Workflow with WfExS started.");

    await TriggerWfexs();
  }

  /// <summary>
  /// Install and run WfExS given 
  /// </summary>
  /// <exception cref="Exception"></exception>
  public async Task TriggerWfexs()
  {
    const string cmd = "bash";
    string activateVenv = "source " + _workflowOptions.VirtualEnvironmentPath;
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
      FileName = cmd,
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
      await streamWriter.WriteLineAsync(activateVenv);
      foreach (var command in commands)
      {
        await streamWriter.WriteLineAsync(command);
      }

      await streamWriter.FlushAsync();
      streamWriter.Close();
    }

    var sb = new StringBuilder();
    StreamReader reader = process.StandardOutput;
    while (!process.HasExited)
      sb.Append(await reader.ReadToEndAsync());
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
