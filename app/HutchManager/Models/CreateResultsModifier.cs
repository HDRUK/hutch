using System.Text.Json;

namespace HutchManager.Models;

public class CreateResultsModifier
{
  public int Id { get; set; }
  public int Order { get; set; }
  public int ActivitySource { get; set; }
  public string Type { get; set; } = string.Empty;
  public JsonDocument? Parameters { get; set; }

  public Data.Entities.ResultsModifier ToEntity(Data.Entities.ModifierType type, Data.Entities.ActivitySource source)
  => new()
  {
    Order = Order,
    Type = type,
    Parameters = Parameters,
    ActivitySource = source
  };
}
