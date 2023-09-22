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
  /// Project Database Credentials or Vault Token.
  /// This allows the requested workflow to be granted access to the
  /// data source it should run against.
  /// </summary>
  public string? DataAccess { get; set; } = string.Empty;
  
  /// <summary>
  /// Absolute URL where the Job's RO-Crate can be fetched from with a GET request.
  ///
  /// If omitted, a later submission can be made to provide a Remote URL or a binary crate payload.
  /// </summary>
  public Uri? CrateUrl { get; set; } = null;
}
