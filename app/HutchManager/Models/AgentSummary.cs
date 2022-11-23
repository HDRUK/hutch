namespace HutchManager.Models;

public class AgentSummary
{
  public int Id { get; set; }

  public string Name { get; set; } = string.Empty;

  public string ClientId { get; set; } = string.Empty;

  public List<string> DataSources { get; set; } = new();
}


