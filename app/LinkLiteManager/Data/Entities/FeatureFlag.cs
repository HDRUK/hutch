using System.ComponentModel.DataAnnotations;

namespace LinkLiteManager.Data.Entities;

public class FeatureFlag
{
  [Key]
  public string Key { get; set; } = string.Empty;
  public bool isActive { get; set; } 

}
