using System.Text.Json;

namespace HutchManager.Models;

public class CreateResultsModifier
{
  public int Id { get; set; }
  public int Order { get; set; }
  public ActivitySourceModel ActivitySource { get; set; } = new();
  public ModifierTypeModel Type { get; set; } = new();
  public JsonDocument? Parameters { get; set; }
}
