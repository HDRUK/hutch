using System.Text.Json.Nodes;

namespace HutchManager.Models;

public class CreateResultsModifier
{
  public int Id { get; set; }
  public int Order { get; set; }
  public ActivitySourceModel ActivitySource { get; set; } = new();
  public ModifierTypeModel Type { get; set; } = new();
  public JsonObject Parameters { get; set; } = new();
}
