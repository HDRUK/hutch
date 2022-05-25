using System.ComponentModel.DataAnnotations;

namespace LinkLiteManager.Models;

public class CreateActivitySource
{
  [Required]
  public int Id { get;set;}
  public string Host { get; set; } 
  public string Type { get; set; }
  public string ResourceId { get; set; }

  public Data.Entities.ActivitySource ToEntity(ActivitySource activitySource) 
    => new()
    {
      Id=Id,
      Host= Host,
      Type = Type,
      ResourceId = ResourceId,
    };

};

