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

  /// <summary>
  /// Parse ROCrate metadata using the ROCrates library
  /// </summary>
  /// <param name="jsonFile"> JSON Metadata file</param>
  /// <returns> ROCrate object </returns>
  private ROCrate ParseCrate(string jsonFile)
  {
    try
    {
      // get metadata from manifest
      var metadataProperties = JsonNode.Parse(jsonFile)?.AsObject();
      metadataProperties.TryGetPropertyValue("@graph", out var graph);
      // get RootDataset Properties and add them to an ROCrates object
      var rootDatasetProperties = graph.AsArray().Where(g => g["@id"].ToString() == "./");
      var datasetRoot = RootDataset.Deserialize(rootDatasetProperties.First().ToString(), _roCrate);

      _roCrate.Add(datasetRoot ?? throw new InvalidOperationException());

      return _roCrate;
    }
    catch
    {
      throw new Exception("Metadata JSON could not be parsed");
    }
  }

  /// <summary>
  /// Unpack an ROCrate
  /// </summary>
  /// <param name="stream"></param>
  /// <exception cref="FileNotFoundException"></exception>
  private void UnpackCrate(Stream stream)
  {
    using var archive = new ZipArchive(stream);
    {
      // Extract to Directory
      archive.ExtractToDirectory(_workflowOptions.CrateExtractPath, true);
      _logger.LogInformation($"ROCrate successfully extracted to {_workflowOptions.CrateExtractPath}");
      // Validate it is an ROCrate
      var file = Directory.GetFiles(_workflowOptions.CrateExtractPath, searchPattern: "ro-crate-metadata.json");
      if (file == null) throw new FileNotFoundException($"No metadata JSON found in directory {_workflowOptions.CrateExtractPath}");
      var fileJson = File.ReadAllText(file[0]);
      // Parse Crate metadata
      var crate = ParseCrate(fileJson);
      // Get mainEntity from metadata
      var mainEntity = crate.RootDataset.GetProperty<Part>("mainEntity");
      var mainEntityPath = Path.Combine(_workflowOptions.CrateExtractPath, mainEntity.Id);
      // Check main entity is present and a stage file
      if (File.Exists(mainEntityPath) && (mainEntityPath.EndsWith(".stage") || mainEntityPath.EndsWith(".yaml") ||
                                          mainEntityPath.EndsWith(".yml")))
      {
        _logger.LogInformation($"main Entity is a Wfexs stage file and can be found at {mainEntityPath}");
        _workflowOptions.StageFilePath = mainEntityPath;
        // Create a copy of the wfexs stage file
        var copyFilePath = Path.Combine(_workflowOptions.CrateExtractPath, "copy_" + mainEntity.Id);
        try
        {
          File.Copy(mainEntityPath,copyFilePath, true);
        }
        // Catch exception if the file was already copied.
        catch (IOException copyError)
        {
          Console.WriteLine(copyError.Message);
        }
        // Rewrite stage file parameter inputs to an absolute path
        // based on "crate" protocol
        using (var stageFileWriter = new StreamWriter(mainEntityPath))
        using (var stageFileReader = new StreamReader(copyFilePath))
        {
          string? line;
          while ((line = stageFileReader.ReadLine()) != null)
          {
            
            if (line.Trim().StartsWith("- crate"))
            {
              _logger.LogInformation($"Found line matching crate protocol {line}");
              stageFileWriter.WriteLine(_rewritePath(line));
            }
            else
            {
              stageFileWriter.WriteLine(line);
            }
            
          }
        }

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

    await using var streamWriter = process.StandardInput;
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

  private string _rewritePath(string? line)
  {
    var newInputPath = line.Split("///");
    
    // keep line whitespaces for yaml formatting purposes
    var newAbsolutePath = newInputPath[0].Split("crate")[0] + "file:///";
    _logger.LogInformation($"Created new input path {newAbsolutePath}");
    var newLine = Path.Combine(newAbsolutePath, _workflowOptions.CrateExtractPath, newInputPath[1]);
    _logger.LogInformation($"Created new input path {newLine}");

    return newLine;
  }
}
