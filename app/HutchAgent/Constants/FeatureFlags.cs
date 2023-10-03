namespace HutchAgent.Constants;

public static class FeatureFlags
{
  /// <summary>
  /// Use Store details from config instead of asking TRE Controller
  /// </summary>
  public const string UsePreconfiguredStore = nameof(UsePreconfiguredStore);
  
  /// <summary>
  /// Operate without connection to a TRE Controller.
  /// Some process steps must be performed manually (e.g. job submissions, egress approval).
  /// <see cref="UsePreconfiguredStore"/> is implicitly true when this is.
  /// Status updates will only be logged locally.
  /// Useful for Development.
  /// </summary>
  public const string StandaloneMode = nameof(StandaloneMode);
}
