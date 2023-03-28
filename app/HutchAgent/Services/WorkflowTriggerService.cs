using System.Diagnostics;
using System.Text;
using HutchAgent.Models;

namespace HutchAgent.Services;

public class WorkflowTriggerService
{
  /// <summary>
  /// Install and run WfExS given 
  /// </summary>
  /// <exception cref="Exception"></exception>
  public void TriggerWfexs(){
    
    const string cmd = "bash";
    const string executionArgs = "";
    const string activateVenv = "source " + WorkflowTriggerOptions.VirtualEnvironmentPath;
    // Commands to install WfExS and execute a workflow
    // given a path to the local config file and a path to the stage file of a workflow
    var commands = new List<string>()
    {
      "apt-get install python3-pip",
      "yes | ./full-installer.bash",
      "yes | ./singularity-local-installer.bash",
      $"./WfExS-backend.py  -L {WorkflowTriggerOptions.LocalConfigPath} execute -W {WorkflowTriggerOptions.StageFilePath}"
    };
    
    var processStartInfo = new ProcessStartInfo
    {
      RedirectStandardOutput = true,
      RedirectStandardInput = true,
      RedirectStandardError = true,
      UseShellExecute = false,
      CreateNoWindow = true,
      Arguments = executionArgs,
      FileName = cmd,
      WorkingDirectory = WorkflowTriggerOptions.ExecutorPath
    };

    var process = Process.Start(processStartInfo);
    if (process == null)
      throw new Exception("Could not start process");

    using var streamWriter = process.StandardInput;
    if (streamWriter.BaseStream.CanWrite)
    {
      // activate python virtual environment
      streamWriter.WriteLine(activateVenv);
      foreach (var command in commands)
      {
        streamWriter.WriteLine(command);
      }
      streamWriter.Flush();
      streamWriter.Close();
    }
    
    var sb = new StringBuilder();
    while (!process.HasExited)
      sb.Append(process.StandardOutput.ReadToEnd());
  }
}
