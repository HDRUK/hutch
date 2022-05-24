namespace LinkLiteManager.Data.Entities;

public class ActivitySource
{
  public int Id { get; set; }

  public string Host { get; set; } = string.Empty;

  public string Type { get; set; } = string.Empty;
  
  public string ResourceId { get; set; } = string.Empty;
  
}
