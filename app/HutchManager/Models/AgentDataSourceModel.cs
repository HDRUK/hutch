using System.Text.Json.Serialization;

namespace HutchManager.Models;

public class AgentDataSource
{
  public int Id { get; set; }

  public string Name { get; set; } = string.Empty;

  public string ClientId { get; set; } = string.Empty;

  public List<string> DataSources { get; set; } = new();

}


