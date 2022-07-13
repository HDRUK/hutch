using System.Text.Json;

namespace HutchManager.Models;

public class ResultsModifierModel
{
  public int Id { get; set; }
  public int Order { get; set; }
  public ActivitySourceModel ActivitySource { get; set; } = new();
  public ModifierTypeModel Type { get; set; } = new();
  public JsonDocument? Parameters { get; set; }

  public ResultsModifierModel(Data.Entities.ResultsModifier entity)
  {
    Id = entity.Id;
    Order = entity.Order;
    ActivitySource = new ActivitySourceModel 
    {
    Id = entity.ActivitySource.Id,
    Host = entity.ActivitySource.Host,
    ResourceId = entity.ActivitySource.ResourceId,
    DisplayName = entity.ActivitySource.DisplayName,
    TargetDataSourceName = entity.ActivitySource.TargetDataSourceName,
    };
    Type = new ModifierTypeModel 
    { 
      Id = entity.Type.Id, 
      Limit = entity.Type.Limit
    };
    Parameters = entity.Parameters;
  }
}
