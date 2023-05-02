using System.Diagnostics;
using System.IO.Compression;
using System.Text;
using System.Text.Json.Nodes;
using HutchAgent.Models;
using Microsoft.Extensions.Options;
using ROCrates;
using ROCrates.Models;
using File = System.IO.File;

namespace HutchAgent.Services;

public class WorkflowTriggerService
{
  private readonly WorkflowTriggerOptions _workflowOptions;
  private readonly ILogger<WorkflowTriggerService> _logger;
  private readonly ROCrate _roCrate = new();
  public WorkflowTriggerService(IOptions<WorkflowTriggerOptions> workflowOptions,
    ILogger<WorkflowTriggerService> logger)
  {
    _logger = logger;
    _workflowOptions = workflowOptions.Value;
  }

  public ROCrate ParseCrate(string jsonFile)
  {
    
      var metadataProperties = JsonNode.Parse(jsonFile)?.AsObject();
      metadataProperties.TryGetPropertyValue("@graph", out var graph);
      var rootDatasetProperties = graph.AsArray().Where(g => g["@id"].ToString() == "./");
      var datasetRoot = RootDataset.Deserialize(rootDatasetProperties.First().ToString(), _roCrate);
      _roCrate.Add(datasetRoot);

      return _roCrate;
  }

  public void UnpackCrate(Stream stream)
  {
      using var archive = new ZipArchive(stream);
      {
        // Extract to Directory
        archive.ExtractToDirectory(_workflowOptions.CrateExtractPath, true);
        
        // Validate it is an ROCrate
        var file = Directory.GetFiles(_workflowOptions.CrateExtractPath, searchPattern:"*metadata.json");
        var fileJson = File.ReadAllText(file[0]);
        // Parse Crate metadata
        var crate = ParseCrate(fileJson);
        var mainEntity = crate.RootDataset.GetProperty<Part>("mainEntity");
        var mainEntityPath = Path.Combine(_workflowOptions.CrateExtractPath, mainEntity.Id);
        // Check main entity is present and a stage file
        if (File.Exists(mainEntityPath) && (mainEntityPath.EndsWith(".stage") || mainEntityPath.EndsWith(".yaml") || mainEntityPath.EndsWith(".yml") ) )
        {
          _workflowOptions.StageFilePath = mainEntityPath;
        }
        else
        {
          throw new FileNotFoundException($"No file named {mainEntity.Id} found in the working directory");
        }
        
      }
    
  }

  /// <summary>
  /// Install and run WfExS given 
  /// </summary>
  /// <param name="stream"></param>
  /// <exception cref="Exception"></exception>
  public async Task TriggerWfexs(Stream stream)
  {
    UnpackCrate(stream);
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
}
