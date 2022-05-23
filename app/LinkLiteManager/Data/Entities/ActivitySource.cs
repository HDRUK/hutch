namespace LinkLiteManager.Data.Entities;

public class ActivitySource
{
  public int Id { get; set; }

  public string Host { get; set; } = string.Empty;

  public SourceType Type { get; set; } = new SourceType();
  
  public string ResourceId { get; set; } = string.Empty;
  
}
