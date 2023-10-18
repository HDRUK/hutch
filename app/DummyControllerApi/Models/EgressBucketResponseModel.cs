namespace DummyControllerApi.Models;

public class EgressBucketResponseModel
{
  public string Host { get; set; } = string.Empty;
  public string Bucket { get; set; } = string.Empty;
  
  /// <summary>
  /// A real TRE Controller API is unlikely to provide this (even though Hutch will accept it)
  /// but since this is for development use, it's the default for us to send it here
  /// </summary>
  public bool Secure { get; set; } = false;
}
