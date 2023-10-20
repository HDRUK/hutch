namespace HutchAgent.Constants;

public static class FeatureFlags
{
  
  /// <summary>
  /// Operate without connection to a TRE Controller.
  /// StoreDefaults will be used for Egress Storage.
  /// Some process steps must be performed manually (e.g. job submissions, egress approval).
  /// Status updates will only be logged locally.
  /// Useful for Development.
  /// </summary>
  public const string StandaloneMode = nameof(StandaloneMode);

  /// <summary>
  /// When possible, retain the working directory and db records for jobs that have failed
  /// </summary>
  public const string RetainFailures = nameof(RetainFailures);
}
