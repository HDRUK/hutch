namespace HutchAgent.Models;

public class JobQueueMessage
{
  public string JobId { get; set; } = string.Empty;

  public string ActionType { get; set; } = string.Empty;
}
