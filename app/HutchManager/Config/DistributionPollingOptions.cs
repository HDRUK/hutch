namespace HutchManager.Config;

public class DistributionPollingOptions
{
  /// <summary>
  /// Polling interval in days
  /// for fetching distribution queries from Activity Sources
  /// </summary>
  public int PollingInterval { get; set; } = 5;
}
