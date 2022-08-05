using System.ComponentModel.DataAnnotations;

namespace HutchManager.Data.Entities;

public class ActivitySource
{
  public int Id { get; set; }

  public string Host { get; set; } = string.Empty;

  [Required]
  public SourceType Type { get; set; } = null!;
  
  public string ResourceId { get; set; } = string.Empty;

  public string DisplayName { get; set; } = string.Empty;
  
  [Required]
  public DataSource TargetDataSource { get; set; } = null!;

}
