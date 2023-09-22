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
  /// Once known, record where the crate came from:
  /// the URL if one was provided; or the original filename if submitted by payload.
  /// </summary>
  public string? CrateSource { get; set; }

  /// <summary>
  /// Access credentials / Vault token for the Data Source the job's workflow interacts with
  /// </summary>
  public string? DataAccess { get; set; }

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

  /// <summary>
  /// Once known, here are the details for egress
  /// </summary>
  public string? EgressTarget { get; set; }

  public DateTimeOffset? ExecutionStartTime { get; set; }
  public DateTimeOffset? EndTime { get; set; }
}

public static class WorkflowJobExtensions
{
  /// <summary>
  /// Determine if this Job has had a Crate submitted.
  /// </summary>
  /// <param name="job"></param>
  /// <returns>Whether the Job has had a Crate submitted.</returns>
  public static bool HasCrateSubmitted(this WorkflowJob job)
    => job.CrateSource is not null;
}
