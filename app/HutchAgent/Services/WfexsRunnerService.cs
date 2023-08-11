using System.Diagnostics;
using System.Text.RegularExpressions;
using HutchAgent.Data.Entities;
using YamlDotNet.RepresentationModel;

namespace HutchAgent.Services;

public class WfexsRunnerService
{
  private string _stageFilePath;
  private string _configFilePath;
  private string _executorDirectory;
  private string _cacheDirectory;
  private WfexsJobService _jobService;

  public WfexsRunnerService(WfexsJobService jobService, string stageFilePath, string configFilePath,
    string executorDirectory, string cacheDirectory)
  {
    _stageFilePath = stageFilePath;
    _configFilePath = configFilePath;
    _jobService = jobService;
    _executorDirectory = executorDirectory;
    _cacheDirectory = cacheDirectory;
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
  /// Check if WfExS jobs are finished and update the database.
  /// </summary>
  public async Task CheckJobsFinished()
  {
    if (_jobService is null) throw new NullReferenceException("_wfexsJobService instance not available");
    var unfinishedJobs = await _jobService.List();
    unfinishedJobs = unfinishedJobs.FindAll(x => !x.RunFinished);

    foreach (var job in unfinishedJobs)
    {
      // 1. find execution-state.yml for job
      var pathToState = Path.Combine(
        _cacheDirectory,
        job.WfexsRunId,
        "meta",
        "execution-state.yaml");
      if (!File.Exists(pathToState)) continue;
      var stateYaml = await File.ReadAllTextAsync(pathToState);
      var configYamlStream = new StringReader(stateYaml);
      var yamlStream = new YamlStream();
      yamlStream.Load(configYamlStream);
      var rootNode = yamlStream.Documents[0].RootNode;

      // 2. get the exit code
      var exitCode = int.Parse(rootNode["exitVal"].ToString());
      job.ExitCode = exitCode;
      // record start and finish times?

      // 3. set job to finished
      job.RunFinished = true;

      // 4. update job in DB
      await _jobService.Set(job);
    }
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
