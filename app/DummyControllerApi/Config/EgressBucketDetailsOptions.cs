namespace DummyControllerApi.Config;

public class EgressBucketDetailsOptions
{
  public string Host { get; set; } = "localhost:9000";
  public string Bucket { get; set; } = "hutch.bucket";

  public string Path { get; set; } = string.Empty;

  public string AccessKey { get; set; } = string.Empty;

  public string SecretKey { get; set; } = string.Empty;
}
