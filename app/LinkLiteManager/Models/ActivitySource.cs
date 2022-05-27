namespace LinkLiteManager.Models;

public class ActivitySource
{

  public int Id { get; set; }
  public string Host { get; set; } = string.Empty;
  public string Type { get; set; } = string.Empty;
  public string ResourceId { get; set; } = string.Empty;

  public ActivitySource(Data.Entities.ActivitySource entity)
  {
    Id = entity.Id;
    Host = entity.Host;
    Type = entity.Type.Id;
    ResourceId = entity.ResourceId;
  }

}

