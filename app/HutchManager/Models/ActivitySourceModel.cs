namespace HutchManager.Models;

public class ActivitySourceModel
{

  public int Id { get; set; }
  public string Host { get; set; } = string.Empty;
  public string Type { get; set; } = string.Empty;
  public string ResourceId { get; set; } = string.Empty;
  public string DisplayName { get; set; } = string.Empty;

  public string TargetDataSourceName { get; set; } = string.Empty;

  public ActivitySourceModel(Data.Entities.ActivitySource entity)
  {
    Id = entity.Id;
    Host = entity.Host;
    Type = entity.Type.Id;
    ResourceId = entity.ResourceId;
    DisplayName = entity.DisplayName;
    TargetDataSourceName = entity.TargetDataSourceName;
  }

  public ActivitySourceModel()
  {
  }
}

