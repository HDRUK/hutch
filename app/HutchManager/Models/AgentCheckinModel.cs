namespace HutchManager.Models;

public class AgentCheckInModel
{
  public List<string> DataSources { get; set; } = new();

  public List<int> Agents { get; set; } = new();
}
