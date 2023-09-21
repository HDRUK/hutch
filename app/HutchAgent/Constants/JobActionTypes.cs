namespace HutchAgent.Constants;

public static class JobActionTypes
{
  /// <summary>
  /// Attempt to fetch a Job's Crate from a remote URL;
  /// if successful Execute its workflow, if possible.
  /// </summary>
  public const string FetchAndExecute = "FetchAndExecute";
  
  /// <summary>
  /// For a Job that already has a Crate, Execute its workflow, if possible.
  /// </summary>
  public const string Execute = "Execute";
  
  /// <summary>
  /// Check if workflow execution is complete
  /// and, if so, co-ordinate with the TRE modules
  /// to start the egress process,
  /// e.g. disclosure checks on outputs.
  /// </summary>
  public const string InitiateEgress = "InitiateEgress";
  
  /// <summary>
  /// After confirmation from the TRE modules,
  /// finalize the crate, package it all up and upload.
  /// </summary>
  public const string Finalize = "Finalize";
  
  /// <summary>
  /// Clean up jobs that never had a Crate successfully submitted
  /// within a reasonable time (e.g. 30days).
  /// </summary>
  public const string Expiry = "Expiry";
}
