using System.ComponentModel.DataAnnotations;

namespace HutchAgent.Models;

public class FileStorageDetails
{
  /// <summary>
  /// Cloud Storage Host e.g. a Min.io Server
  /// </summary>
  [Required] public string Host { get; set; } = string.Empty;

  /// <summary>
  /// Cloud Storage Container name e.g. a Min.io Bucket
  /// </summary>
  [Required] public string Bucket { get; set; } = string.Empty;

  /// <summary>
  /// Object ID for a single Cloud Storage item in the named <see cref="Bucket"/>,
  /// functionally the "Path" to the stored file.
  /// </summary>
  [Required] public string Path { get; set; } = string.Empty;
}
