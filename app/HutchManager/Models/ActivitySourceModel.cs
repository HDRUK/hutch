namespace HutchManager.Models;

public class ActivitySourceModel
{
  public int Id { get; set; }
  public string Host { get; set; }
  public string Type { get; set; }
  public string ResourceId { get; set; }
  public string DisplayName { get; set; }

  public string TargetDataSource { get; set; }

  public ActivitySourceModel(Data.Entities.ActivitySource entity)
  {
    Id = entity.Id;
    Host = entity.Host;
    Type = entity.Type.Id;
    ResourceId = entity.ResourceId;
    DisplayName = entity.DisplayName;
    TargetDataSource = entity.TargetDataSource.Id;
  }
}

