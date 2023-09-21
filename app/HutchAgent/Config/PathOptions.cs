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

/// <summary>
/// Extension methods to avoid doing the same path concatenations over and over in different places ;)
/// </summary>
public static class PathOptionsExtensions
{
  /// <summary>
  /// Calculates a Job's working directory based on current Hutch config.
  /// </summary>
  /// <param name="paths">PathOptions, typically injected via IOptions</param>
  /// <param name="jobId">The ID of the Job</param>
  /// <returns></returns>
  public static string JobWorkingDirectory(this PathOptions paths, string jobId)
    => Path.Combine(paths.WorkingDirectoryBase, paths.Jobs, jobId);
}

