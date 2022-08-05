using System.Text.Json;

namespace HutchManager.Models;

public class ResultsModifierModel
{
  public int Id { get; set; }
  public int Order { get; set; }
  public ActivitySourceModel ActivitySource { get; set; }
  public ModifierTypeModel Type { get; set; }
  public JsonDocument? Parameters { get; set; }

  public ResultsModifierModel(Data.Entities.ResultsModifier entity)
  {
    Id = entity.Id;
    Order = entity.Order;
    ActivitySource = new(entity.ActivitySource);
    Type = new(entity.Type);
    Parameters = entity.Parameters;
  }
}
