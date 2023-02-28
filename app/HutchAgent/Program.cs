using System.Diagnostics;
using HutchAgent.Models;
using HutchAgent.Services;

namespace HutchAgent;
class Program
{  
    // private static readonly WorkflowTriggerService _workflowTriggerService;
   
    static void Main(string[] args)
    {
      string directory = Directory.GetCurrentDirectory();
      Console.WriteLine(directory);
      const string cmd = "bash";
      const string executionArgs = "";
      const string activateVenv = "source" + WorkflowTriggerOptions.VirtualEnvironmentPath ;
      var commandsToExecute = new List<string>(){
        "pip install --upgrade pip wheel",
        "pip install -r requirements.txt",
        "pip install -r dev-requirements.txt",
        "pip install -r mypy-requirements.txt",
        "./full-installer.bash",
        "python WfExS-backend.py --full-help"
      };
      
      var startInfo = new ProcessStartInfo
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
