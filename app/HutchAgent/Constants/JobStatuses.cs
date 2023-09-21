namespace HutchAgent.Constants;

public enum JobStatus
{
  /// <summary>
  /// Waiting for a Crate payload or Crate URL to be submitted to Hutch.
  /// </summary>
  WaitingForCrate,
  
  /// <summary>
  /// A Crate has been submitted via URL; Hutch is fetching it.
  /// </summary>
  FetchingCrate,
  
  /// <summary>
  /// Hutch has queued the Crate to be executed.
  /// </summary>
  Queued,
  
  /// <summary>
  /// Hutch is validating the Job Crate metadata prior to execution.
  /// </summary>
  ValidatingCrate,
  
  /// <summary>
  /// Hutch is fetching the remote workflow referenced by the crate.
  /// </summary>
  FetchingWorkflow,
  
  /// <summary>
  /// Hutch is preparing the workflow for the Workflow Executor (e.g. WfExS)
  /// using a combination of the Job Crate, the Workflow Crate and Executor configuration.
  /// </summary>
  StagingWorkflow,
  
  /// <summary>
  /// Hutch has triggered the Workflow Executor to run the Worflow.
  /// </summary>
  ExecutingWorkflow,
  
  /// <summary>
  /// Following successful Workflow Execution, Hutch is preparing outputs
  /// for an Egress request (e.g. disclosure checks etc.)
  /// </summary>
  PreparingOutputs,
  
  /// <summary>
  /// Hutch has requested Egress and/or is providing outputs for Egress checking
  /// </summary>
  DataOutRequested,
  
  /// <summary>
  /// Hutch has provided outputs for Egress and is awaiting the results of checks
  /// </summary>
  TransferredForDataOut,

  /// <summary>
  /// Egress checks have approved some or all outputs; Hutch is finalising the approved results RO-Crate package.
  /// </summary>
  PackagingApprovedResults,
  
  /// <summary>
  /// Hutch has transferred the results RO-Crate package to the Intermediary Store.
  /// </summary>
  Complete,
  
  /// <summary>
  /// Hutch was unable to get the job to a complete status; it failed during the previous status stage.
  /// </summary>
  Failure
}
