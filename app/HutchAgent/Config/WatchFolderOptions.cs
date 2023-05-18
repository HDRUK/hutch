namespace HutchAgent.Config;

public class WatchFolderOptions
{
  public string Path { get; set; } = string.Empty;
  public int PollingIntervalSeconds { get; set; } = 5;
}
