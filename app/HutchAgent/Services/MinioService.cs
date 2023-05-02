using HutchAgent.Config;
using Microsoft.Extensions.Options;
using Minio;
using Minio.Exceptions;

namespace HutchAgent.Services;

public class MinioService
{
  private readonly MinioClient _minioClient;
  private readonly ILogger<MinioService> _logger;
  private readonly MinioOptions _options;

  public MinioService(IOptions<MinioOptions> minioOptions, ILogger<MinioService> logger, IOptions<MinioOptions> options)
  {
    _logger = logger;
    _options = options.Value;
    _minioClient = new MinioClient()
      .WithEndpoint(_options.Endpoint)
      .WithCredentials(_options.AccessKey, _options.SecretKey)
      .WithSSL(_options.Secure)
      .Build();
  }

  /// <summary>
  /// Upload a file to an S3 bucket.
  /// </summary>
  /// <param name="filePath">The path of the file to be uploaded.</param>
  /// <exception cref="BucketNotFoundException">Thrown when the given bucket doesn't exists.</exception>
  /// <exception cref="MinioException">Thrown when any other error related to MinIO occurs.</exception>
  /// <exception cref="FileNotFoundException">Thrown when the file to be uploaded does not exist.</exception>
  public async Task UploadToBucket(string filePath)
  {
    var exists = await _bucketExists();
    if (!exists) throw new BucketNotFoundException(_options.BucketName, $"No such bucket: {_options.BucketName}");

    if (!System.IO.File.Exists(filePath)) throw new FileNotFoundException();

    var objectName = System.IO.Path.GetFileName(filePath);
    var putObjectArgs = new PutObjectArgs()
      .WithBucket(_options.BucketName)
      .WithFileName(filePath)
      .WithObject(objectName);

    _logger.LogInformation($"Uploading {objectName} to {_options.BucketName}...");
    await _minioClient.PutObjectAsync(putObjectArgs);
    _logger.LogInformation($"Successfully uploaded {objectName} to {_options.BucketName}.");
  }

  /// <summary>
  /// Check if a file already exists in an S3 bucket.
  /// </summary>
  /// <param name="filePath">The name of the file to check in the bucket.</param>
  /// <exception cref="BucketNotFoundException">Thrown when the given bucket doesn't exists.</exception>
  /// <exception cref="MinioException">Thrown when any other error related to MinIO occurs.</exception>
  public async Task<bool> FileExistsInBucket(string filePath)
  {
    if (!await _bucketExists())
      throw new BucketNotFoundException(_options.BucketName, $"No such bucket: {_options.BucketName}");

    var statObjectArgs = new StatObjectArgs()
      .WithBucket(_options.BucketName)
      .WithObject(filePath);

    _logger.LogInformation($"Looking for {filePath} in {_options.BucketName}...");
    var stat = await _minioClient.StatObjectAsync(statObjectArgs);
    if (stat is not null)
    {
      _logger.LogInformation($"Found {filePath} in {_options.BucketName}.");
      return true;
    }

    _logger.LogInformation($"Could not find {filePath} in {_options.BucketName}.");
    return false;
  }

  private async Task<bool> _bucketExists()
  {
    var args = new BucketExistsArgs().WithBucket(_options.BucketName);
    return await _minioClient.BucketExistsAsync(args);
  }
}
