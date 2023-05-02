namespace HutchAgent.Config;

public abstract class WatchFolderOptions
{
  public string Path { get; set; } = string.Empty;
  public double PollingIntervalSeconds { get; set; } = 5.0;
}
