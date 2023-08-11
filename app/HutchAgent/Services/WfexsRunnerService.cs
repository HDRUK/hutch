using System.Diagnostics;
using System.Text.RegularExpressions;
using HutchAgent.Data.Entities;

namespace HutchAgent.Services;

public class WfexsRunnerService
{
  private string _stageFilePath;
  private string _configFilePath;
  private string _executorDirectory;
  private WfexsJobService _jobService;

  public WfexsRunnerService(WfexsJobService jobService, string stageFilePath, string configFilePath,
    string executorDirectory)
  {
    _stageFilePath = stageFilePath;
    _configFilePath = configFilePath;
    _jobService = jobService;
    _executorDirectory = executorDirectory;
  }

  /// <summary>
  /// Run WfExS with the given config and stage files.
  /// </summary>
  /// <exception cref="Exception">WfExS could not be executed.</exception>
  public async Task RunWfexs()
  {
    var job = new WfexsJob();
    // Commands to install WfExS and execute a workflow
    // given a path to the local config file and a path to the stage file of a workflow
    var processStartInfo = new ProcessStartInfo
    {
      RedirectStandardOutput = true,
      RedirectStandardInput = true,
      RedirectStandardError = true,
      UseShellExecute = false,
      CreateNoWindow = true,
      FileName = "./WfExS-backend.py",
      Arguments = $"-L {_configFilePath} execute -W {_stageFilePath}",
      WorkingDirectory = _executorDirectory
    };

    // start process
    var process = Process.Start(processStartInfo);
    if (process == null)
      throw new Exception("Could not start process");

    // Get process PID
    job.Pid = process.Id;

    // Read the stdout of the WfExS run to get the run ID
    var reader = process.StandardOutput;
    while (!process.HasExited && string.IsNullOrEmpty(job.WfexsRunId))
    {
      var stdOutLine = await reader.ReadLineAsync();
      if (stdOutLine is null) continue;
      var runName = _findRunName(stdOutLine);
      if (runName is null) continue;
      job.WfexsRunId = runName;
    }

    // close our connection to the process
    process.Close();

    // Add the job in the queue.
    await _jobService.Create(job);
  }

  /// <summary>
  /// Extract the UUID representing a WfExS run from a line of text.
  /// </summary>
  /// <param name="text">The text to search for the run ID.</param>
  /// <returns>The UUID if found, else <c>null</c>.</returns>
  private string? _findRunName(string text)
  {
    var pattern =
      @".*-\sInstance\s([0-9a-fA-F]{8}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{12}).*";
    var regex = new Regex(pattern);

    var match = regex.Match(text);
    if (!match.Success)
    {
      return null;
    }

    // Get the matched UUID pattern
    var uuid = match.Groups[1].Value;
    return Guid.TryParse(uuid, out var validUuid) ? validUuid.ToString() : null;
  }
}
