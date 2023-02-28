using System.Diagnostics;
using HutchAgent.Models;

namespace HutchAgent.Services;

public class WorkflowTriggerService
{
  public void TriggerWfexs(){
    
    const string cmd = "bash";
    const string args = "";
    const string activateVenv = "source" + WorkflowTriggerOptions.VirtualEnvironmentPath ;
    var commandsToExecute = new List<string>(){
      "pip install --upgrade pip wheel",
      "pip install -r requirements.txt",
      "pip install -r dev-requirements.txt",
      "pip install -r mypy-requirements.txt",
      "python /path/to/script arg1 arg2 arg3"
    };

    var startInfo = new ProcessStartInfo
    {
      RedirectStandardOutput = true,
      RedirectStandardInput = true,
      RedirectStandardError = true,
      UseShellExecute = false,
      CreateNoWindow = true,
      Arguments = args,
      FileName = cmd,
      WorkingDirectory = WorkflowTriggerOptions.ExecutorPath
    };

    var process = Process.Start(startInfo);
    if (process == null)
      throw new Exception("Could not start process");

    using var streamWriter = process.StandardInput;
    if (streamWriter.BaseStream.CanWrite)
    {
      streamWriter.WriteLine(activateVenv);
      foreach (var command in commandsToExecute)
      {
        streamWriter.WriteLine(command);
      }
      streamWriter.Flush();
      streamWriter.Close();
    }
  }
}
