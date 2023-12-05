namespace DummyControllerApi.Models.WebHooks;

public class FinalOutcomeWebHookModel
{
  /// <summary>
  /// Submission ID
  /// </summary>
  public string SubId { get; set; } = string.Empty;

  /// <summary>
  /// The Minio Host details
  /// </summary>
  public string Host { get; set; } = string.Empty;

  /// <summary>
  /// The name of the target Minio bucket
  /// </summary>
  public string Bucket { get; set; } = string.Empty;

  /// <summary>
  /// A path to prefix to object id's uploaded for this submission
  /// </summary>
  public string File { get; set; } = string.Empty;

  /// <summary>
  /// A real TRE Controller API is unlikely to provide this (even though Hutch will accept it)
  /// but since this is for development use, it's the default for us to send it here
  /// </summary>
  public bool Secure { get; set; } = false;

  /// <summary>
  /// Access Key for Cloud Storage. If omitted, will use OIDC if configured, or default to preconfigured value if not.
  /// </summary>
  public string AccessKey { get; set; } = string.Empty;

  /// <summary>
  /// Secret Key for Cloud Storage. If omitted, will use OIDC if configured, or default to preconfigured value if not.
  /// </summary>
  public string SecretKey { get; set; } = string.Empty;
}
