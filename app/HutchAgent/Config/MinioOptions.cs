namespace HutchAgent.Config;

public class MinioOptions
{
  public string Endpoint { get; set; } = "localhost:9000";
  public string AccessKey { get; set; } = string.Empty;
  public string SecretKey { get; set; } = string.Empty;
  public bool Secure { get; set; } = false;
  public string BucketName { get; set; } = string.Empty;
}
