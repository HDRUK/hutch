using HutchAgent.Config;
using Microsoft.Extensions.Options;
using Minio;

namespace HutchAgent.Services;

public class MinioService
{
  private MinioClient _minioClient;

  public MinioService(IOptions<MinioOptions> minioOptions)
  {
    _minioClient = new MinioClient()
      .WithEndpoint(minioOptions.Value.Endpoint)
      .WithCredentials(minioOptions.Value.AccessKey, minioOptions.Value.SecretKey)
      .WithSSL(minioOptions.Value.Secure)
      .Build();
  }
}
