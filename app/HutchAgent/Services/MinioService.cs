using HutchAgent.Config;
using Microsoft.Extensions.Options;
using Minio;
using Minio.Exceptions;

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

  /// <summary>
  /// Upload a file to an S3 bucket.
  /// </summary>
  /// <param name="bucketName">The name of the bucket to which the file will be uploaded.</param>
  /// <param name="filePath">The path of the file to be uploaded.</param>
  /// <exception cref="BucketNotFoundException">Thrown when the given bucket doesn't exists.</exception>
  /// <exception cref="MinioException">Thrown when any other error related to MinIO occurs.</exception>
  /// <exception cref="FileNotFoundException">Thrown when the file to be uploaded does not exist.</exception>
  public async Task UploadToBucket(string bucketName, string filePath)
  {
    var exists = await _bucketExists(bucketName);
    if (!exists) throw new BucketNotFoundException(bucketName, $"No such bucket: {bucketName}");

    if (!System.IO.File.Exists(filePath)) throw new FileNotFoundException();

    var putObjectArgs = new PutObjectArgs()
      .WithBucket(bucketName)
      .WithFileName(filePath)
      .WithObject(System.IO.Path.GetFileName(filePath));
    await _minioClient.PutObjectAsync(putObjectArgs);
  }

  private async Task<bool> _bucketExists(string bucketName)
  {
    var args = new BucketExistsArgs().WithBucket(bucketName);
    return await _minioClient.BucketExistsAsync(args);
  }
}
