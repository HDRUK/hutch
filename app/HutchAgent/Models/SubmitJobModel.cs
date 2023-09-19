using System.ComponentModel.DataAnnotations;

namespace HutchAgent.Models;

public class JobOutputStoreModel
{
  /// <summary>
  /// Absolute URL to where outputs should be stored.
  ///
  /// Can be a Minio / S3 URL, or a filepath.
  /// </summary>
  public Uri? Url { get; set; } = null;

  /// <summary>
  /// Minio / S3 Access token if required.
  /// </summary>
  public string? AccessToken { get; set; } = null;
}

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
  /// The name of the Project the job is for, as configured in the Submission Layer.
  /// </summary>
  [Required]
  public string ProjectName { get; set; } = string.Empty;

  /// <summary>
  /// The TRE's identifier for the project, required for interaction with the TRE Agent.
  /// </summary>
  [Required]
  public string ProjectId { get; set; } = string.Empty;

  /// <summary>
  /// Project Database Credentials or Vault Token.
  /// This allows the requested workflow to be granted access to the
  /// data source it should run against.
  /// </summary>
  public string? DataAccess { get; set; } = string.Empty;

  public JobOutputStoreModel OutputStore { get; set; } = new();

  /// <summary>
  /// Absolute URL where the Job's RO-Crate can be fetched from,
  /// if not provided directly in the payload via <see cref="Crate"/>.
  /// </summary>
  public Uri? CrateUrl { get; set; } = null;

  /// <summary>
  /// The Job RO-Crate. Can be ommitted if <see cref="CrateUrl"/>
  /// provides one instead.
  /// </summary>
  public IFormFile? Crate { get; set; } = null;
}
