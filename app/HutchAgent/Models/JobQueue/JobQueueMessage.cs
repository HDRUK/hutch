namespace HutchAgent.Models.JobQueue;

public class JobQueueMessage
{
  public string JobId { get; set; } = string.Empty;

  public string ActionType { get; set; } = string.Empty;
}

public class JobQueueMessage<T> : JobQueueMessage
  where T : class, new()
{
  public T Payload { get; set; } = new();
}
