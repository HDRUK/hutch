using System.Text.Json.Serialization;

namespace HutchManager.Data.Entities;

public class DataSource
{
  public string Id { get; set; } = string.Empty;
  public DateTimeOffset LastCheckin { get; set; }

  [JsonIgnore]
  public List<Agent> Agents { get; set; } = new();
}
