namespace LinkLiteManager.Data.Entities;

public class ActivitySource
{
  public int Id { get; set; }

  public string Host { get; set; }

  public SourceType Type { get; set; }
  
  public string ResourceId { get; set; } 
  
}
