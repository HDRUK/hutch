using System.Text.Json;

namespace HutchManager.Models;

public class CreateResultsModifier
{
  public int Id { get; set; }
  public int Order { get; set; }
  public string ActivitySource { get; set; } = string.Empty;
  public string Type { get; set; } = string.Empty;
  public JsonDocument? Parameters { get; set; }

  public Data.Entities.ResultsModifier ToEntity(Data.Entities.ModifierType type)
  => new()
  {
    Order = Order,
    Type = type,
    Parameters = Parameters,
  };
}
