namespace HutchAgent.Constants;

/// <summary>
/// Enumeration of Job Statuses that Hutch will report.
/// Values here correspond to a subset of Statuses in the TRE Controller modules and must match.
/// </summary>
public enum JobStatus
{
  /// <summary>
  /// Waiting for a Crate payload or Crate URL to be submitted to Hutch.
  /// </summary>
  WaitingForCrate = 30,
  
  /// <summary>
  /// A Crate has been submitted via URL; Hutch is fetching it.
  /// </summary>
  FetchingCrate = 31,
  
  /// <summary>
  /// Hutch has queued the Crate to be executed.
  /// </summary>
  Queued = 32,
  
  /// <summary>
  /// Hutch is validating the Job Crate metadata prior to execution.
  /// </summary>
  ValidatingCrate = 33,
  
  /// <summary>
  /// Hutch is fetching the remote workflow referenced by the crate.
  /// </summary>
  FetchingWorkflow = 34,
  
  /// <summary>
  /// Hutch is preparing the workflow for the Workflow Executor (e.g. WfExS)
  /// using a combination of the Job Crate, the Workflow Crate and Executor configuration.
  /// </summary>
  StagingWorkflow = 35,
  
  /// <summary>
  /// Hutch has triggered the Workflow Executor to run the Worflow.
  /// </summary>
  ExecutingWorkflow = 36,
  
  /// <summary>
  /// Following successful Workflow Execution, Hutch is preparing outputs
  /// for an Egress request (e.g. disclosure checks etc.)
  /// </summary>
  PreparingOutputs = 37,
  
  /// <summary>
  /// Hutch has requested Egress and/or is providing outputs for Egress checking
  /// </summary>
  DataOutRequested = 38,
  
  /// <summary>
  /// Hutch has provided outputs for Egress and is awaiting the results of checks
  /// </summary>
  TransferredForDataOut = 39,

  /// <summary>
  /// Egress checks have approved some or all outputs; Hutch is finalising the approved results RO-Crate package.
  /// </summary>
  PackagingApprovedResults = 40,
  
  /// <summary>
  /// Hutch has transferred the results RO-Crate package to the Intermediary Store.
  /// </summary>
  Complete = 41,
  
  /// <summary>
  /// Hutch was unable to get the job to a complete status; it failed during the previous status stage.
  /// </summary>
  Failure = 42
}
