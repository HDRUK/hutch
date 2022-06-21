using System.ComponentModel.DataAnnotations;

namespace HutchManager.Models;

public class CreateActivitySource
{
  [Required]
  public string Host { get; set; } = string.Empty;
  [Required]
  public string Type { get; set; } = string.Empty;
  [Required]
  public string ResourceId { get; set; } = string.Empty;
  [Required]
  public string DisplayName { get; set; } = string.Empty;

  public Data.Entities.ActivitySource ToEntity(List<Data.Entities.SourceType> types)
    => new()
    {
      Host = Host,
      Type = types.FirstOrDefault(x=>x.Id==Type)??
      throw new InvalidOperationException($"Type {Type} is not a valid SourceType"),
      ResourceId = ResourceId
    };

}


