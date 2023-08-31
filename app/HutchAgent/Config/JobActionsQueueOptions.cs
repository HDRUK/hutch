namespace HutchAgent.Config;

public class JobActionsQueueOptions
{
  public string QueueName { get; set; } = "WorkflowJobActions";
  
  public int PollingIntervalSeconds { get; set; } = 5;

  public int MaxParallelism { get; set; } = 10; // TODO: can we base this on Environment?
}
