namespace HutchManager.Data.Entities;

using System.ComponentModel.DataAnnotations;

public class Agent
{
  public int Id { get; set; }

  public string Name { get; set; } = string.Empty;

  [Required] public string ClientId { get; set; } = string.Empty;

  [Required] public string ClientSecretHash { get; set; } = string.Empty;

  public List<DataSource> DataSources { get; set; } = null!;
}
