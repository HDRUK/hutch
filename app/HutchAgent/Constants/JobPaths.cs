using HutchAgent.Services;

namespace HutchAgent.Constants;

/// <summary>
/// All relative paths to a job's working directory
/// </summary>
public static class JobPaths
{
  /// <summary>
  /// path to the unpacked 5-Safes RO-Crate BagIt as Hutch actively works on it
  /// </summary>
  public const string BagItRoot = "work";

  /// <summary>
  /// A temp scratch path for whatever's needed,
  /// e.g. downloading remote job crates pre-unpacking.
  /// </summary>
  public const string Temp = "temp";

  /// <summary>
  /// For preparing outputs for Egress Request.
  /// </summary>
  public const string EgressOutputs = "outputs";

  /// <summary>
  /// For the final published package (5 Safes RO-Crate) to be published.
  /// This will contain a zipped artifact of the final state of the <see cref="BagItRoot" /> directory.
  /// </summary>
  public const string EgressPackage = "publish";
}

/// <summary>
/// Extensions to get to common Job subdirectories, based on a Job's stored working directory root
/// </summary>
public static class JobPathsExtensions
{
  public static string JobTemp(this string jobWorkingDirectory)
    => Path.Combine(jobWorkingDirectory, JobPaths.Temp);

  public static string JobBagItRoot(this string jobWorkingDirectory)
    => Path.Combine(jobWorkingDirectory, JobPaths.BagItRoot);

  public static string JobCrateRoot(this string jobWorkingDirectory)
    => Path.Combine(jobWorkingDirectory, JobPaths.BagItRoot).BagItPayloadPath();

  public static string JobEgressOutputs(this string jobWorkingDirectory)
    => Path.Combine(jobWorkingDirectory, JobPaths.EgressOutputs);

  public static string JobEgressPackage(this string jobWorkingDirectory)
    => Path.Combine(jobWorkingDirectory, JobPaths.EgressPackage);
}
