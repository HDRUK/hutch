using System.ComponentModel.DataAnnotations;

namespace HutchManager.Data.Entities;

public class FeatureFlag
{
  [Key]
  public string Key { get; set; } = string.Empty;
  public bool isActive { get; set; } 

}
