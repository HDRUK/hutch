namespace DummyControllerApi.Models;

public class EgressBucketResponseModel
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

  public string Path { get; set; } = string.Empty;
  
  /// <summary>
  /// A real TRE Controller API is unlikely to provide this (even though Hutch will accept it)
  /// but since this is for development use, it's the default for us to send it here
  /// </summary>
  public bool Secure { get; set; } = false;
}
