using System.Text.Json.Nodes;

namespace HutchManager.Models;

public class ResultsModifierModel
{
  public int Id { get; set; }
  public int Order { get; set; }
  public ActivitySourceModel ActivitySource { get; set; } = new();
  public ModifierTypeModel Type { get; set; } = new();

  public JsonObject Parameters { get; set; } = new();

  public ResultsModifierModel(Data.Entities.ResultsModifier entity)
  {
    Id = entity.Id;
    Order = entity.Order;
    ActivitySource = entity.ActivitySource;
    Type = entity.Type;
    Parameters = entity.Parameters;
  }
}
