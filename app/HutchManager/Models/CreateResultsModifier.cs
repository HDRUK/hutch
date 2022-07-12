using System.Text.Json;

namespace HutchManager.Models;

public class CreateResultsModifier
{
  public int Id { get; set; }
  public int Order { get; set; }
  public ActivitySourceModel ActivitySource { get; set; } = new();
  public string Type { get; set; } = string.Empty;
  public JsonDocument? Parameters { get; set; }
}
