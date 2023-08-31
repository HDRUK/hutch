namespace HutchAgent.Models;

/// <summary>
/// Represents the state of a requested job for running a workflow.
/// </summary>
public class WorkflowJob
{
  /// <summary>
  /// Job ID - provided from an external source when the job is submitted
  /// </summary>
  public string Id { get; set; } = string.Empty;

  /// <summary>
  /// Absolute path to Hutch's working directory for this Job
  /// </summary>
  public string WorkingDirectory { get; set; } = string.Empty;

  /// <summary>
  /// The ID of the Workflow Run as provided by the Workflow Executor (e.g. Wfexs)
  /// </summary>
  public string ExecutorRunId { get; set; } = string.Empty;

  /// <summary>
  /// Exit Code of the Workflow Executor Run
  /// </summary>
  public int? ExitCode { get; set; }

  public DateTimeOffset? ExecutionStartTime { get; set; }
  public DateTimeOffset? EndTime { get; set; }
}
