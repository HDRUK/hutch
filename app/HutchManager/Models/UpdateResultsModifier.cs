using System.Text.Json;

namespace HutchManager.Models;

public class UpdateResultsModifier
{
  public int ActivitySourceId { get; set; }
  public string Type { get; set; } = string.Empty;
  public JsonDocument? Parameters { get; set; }

  public Data.Entities.ResultsModifier ToEntity(Data.Entities.ModifierType type, Data.Entities.ActivitySource source)
    => new()
    {
      Type = type,
      Parameters = Parameters,
      ActivitySource = source
    };
}
