using System.ComponentModel.DataAnnotations;

namespace HutchManager.Models;

public class CreateActivitySource
{
  [Required] public string Host { get; set; } = string.Empty;
  [Required] public string Type { get; set; } = string.Empty;
  [Required] public string ResourceId { get; set; } = string.Empty;
  [Required] public string DisplayName { get; set; } = string.Empty;
  [Required] public string TargetDataSource { get; set; } = string.Empty;

  public Data.Entities.ActivitySource ToEntity(List<Data.Entities.SourceType> types,
    List<Data.Entities.DataSource> dataSources)
    => new()
    {
      Host = Host,
      Type = types.FirstOrDefault(x => x.Id == Type) ??
             throw new InvalidOperationException($"Type {Type} is not a valid SourceType"),
      ResourceId = ResourceId,
      DisplayName = DisplayName,
      TargetDataSource = dataSources.SingleOrDefault(x => x.Id == TargetDataSource) ??
                         throw new InvalidOperationException(
                           $"DataSource {TargetDataSource} is not a valid DataSource"),
    };
}
