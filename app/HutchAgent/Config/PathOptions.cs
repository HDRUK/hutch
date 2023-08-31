namespace HutchAgent.Config;

public class PathOptions
{
  /// <summary>
  /// An absolute path for Hutch to use as a working directory.
  /// </summary>
  public string WorkingDirectoryBase { get; set; } = string.Empty;

  /// <summary>
  /// <para>A Path to where job working directories should be.</para>
  /// <para>Relative paths will be appended to <see cref="WorkingDirectoryBase"/>.</para>
  /// </summary>
  public string Jobs { get; set; } = "jobs";
}
