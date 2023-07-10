using System.Text.Json;
using ROCrates.Models;

namespace HutchAgent.Services;

/// <summary>
/// A service for writing WfExS stage files.
/// </summary>
public class StageFileService
{
  private readonly ILogger<StageFileService> _logger;

  public StageFileService(ILogger<StageFileService> logger)
  {
    _logger = logger;
  }

  /// <summary>
  /// Write a WfExS stage file based on the main entity of a WorkflowHub workflow.
  /// </summary>
  /// <param name="mainEntity"></param>
  /// <param name="stageFileName"></param>
  public void WriteStageFile(Entity mainEntity, string stageFileName)
  {
    mainEntity.Properties.TryGetPropertyValue("inputs", out var jsonInputs);
    mainEntity.Properties.TryGetPropertyValue("outputs", out var jsonOutputs);
    if (jsonInputs is null) _logger.LogInformation("No inputs found for main entity.");
    try
    {
      var inputs = jsonInputs.Deserialize<List<Part>>();
      foreach (var input in inputs!)
      {
        
      }
    }
    catch (JsonException)
    {
      _logger.LogError("Could not parse");
    }
    
  }
}
