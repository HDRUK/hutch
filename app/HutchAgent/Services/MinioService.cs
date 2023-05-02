using HutchAgent.Config;
using Microsoft.Extensions.Options;
using Minio;
using Minio.Exceptions;

namespace HutchAgent.Services;

public class MinioService
{
  private readonly MinioClient _minioClient;
  private readonly ILogger<MinioService> _logger;

  public MinioService(IOptions<MinioOptions> minioOptions, ILogger<MinioService> logger)
  {
    _logger = logger;
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

    var objectName = System.IO.Path.GetFileName(filePath);
    var putObjectArgs = new PutObjectArgs()
      .WithBucket(bucketName)
      .WithFileName(filePath)
      .WithObject(objectName);

    _logger.LogInformation($"Uploading {objectName} to {bucketName}...");
    await _minioClient.PutObjectAsync(putObjectArgs);
    _logger.LogInformation($"Successfully uploaded {objectName} to {bucketName}.");
  }

  private async Task<bool> _bucketExists(string bucketName)
  {
    var args = new BucketExistsArgs().WithBucket(bucketName);
    return await _minioClient.BucketExistsAsync(args);
  }
}
