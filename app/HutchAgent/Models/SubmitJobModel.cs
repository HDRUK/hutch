using System.ComponentModel.DataAnnotations;

namespace HutchAgent.Models;

public class SubmitJobModel
{
  /// <summary>
  /// This is a Job ID as provided by the TRE Agent.
  /// Hutch continues to use it to identify the job internally,
  /// but also to interact with the TRE Agent in future.
  /// </summary>
  [Required]
  public string JobId { get; set; } = string.Empty;

  /// <summary>
  /// Optional Project Database Connection details.
  /// This allows the requested workflow to be granted access to the
  /// data source it should run against, if any.
  /// </summary>
  public DatabaseConnectionDetails? DataAccess { get; set; }

  /// <summary>
  /// <para>
  /// Optional absolute URL where the Job's RO-Crate can be fetched from with a GET request.
  /// </para>
  ///
  /// <para>Mutually exclusive with <see cref="CrateUrl"/></para>
  ///
  /// <para>
  /// If all crate values are omitted at the time of Submission,
  /// a later submission can be made to provide a Remote URL, Cloud Storage details, or a binary crate payload.
  /// </para>
  /// </summary>
  public Uri? CrateUrl { get; set; }

  /// <summary>
  /// <para>
  /// Optional details for where the crate can be found in a Cloud Storage Provider.
  /// </para>
  ///
  /// <para>Mutually exclusive with <see cref="CrateUrl"/></para>
  ///
  /// <para>
  /// If all crate values are omitted at the time of Submission,
  /// a later submission can be made to provide a Remote URL, Cloud Storage details, or a binary crate payload.
  /// </para>
  /// </summary>
  public FileStorageDetails? CrateSource { get; set; }
}
