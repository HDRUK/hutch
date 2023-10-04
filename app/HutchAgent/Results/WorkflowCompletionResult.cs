namespace HutchAgent.Results;

/// <summary>
/// Describes whether or not a Workflow Execution is Complete,
/// along with some additional metadata, conditional upon whether it's complete.
/// </summary>
public class WorkflowCompletionResult
{
  /// <summary>
  /// Is the Workflow Execution Complete?
  /// </summary>
  public bool IsComplete { get; set; }

  /// <summary>
  /// If complete, what was the exit code, if known.
  /// </summary>
  public int? ExitCode { get; set; }

  /// <summary>
  /// What time did the execution start, if known.
  /// </summary>
  public DateTimeOffset? StartTime { get; set; }
  
  /// <summary>
  /// If complete, what time did execution end, if known.
  /// </summary>
  public DateTimeOffset? EndTime { get; set; }
}
