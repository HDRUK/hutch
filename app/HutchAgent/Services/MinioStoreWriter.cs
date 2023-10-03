using HutchAgent.Config;
using Microsoft.Extensions.Options;
using Minio;
using Minio.Exceptions;

namespace HutchAgent.Services;

public class MinioStoreWriter
{
  private readonly ILogger<MinioStoreWriter> _logger;
  private MinioClient _minioClient = null!; // indirectly init in ctor
  private MinioOptions _options = null!; // indirectly init in ctor

  public MinioStoreWriter(
    ILogger<MinioStoreWriter> logger,
    IOptions<MinioOptions> options)
  {
    _logger = logger;
    UseOptions(options.Value);
  }

  /// <summary>
  /// Replace the pre-configured store options with a specific set
  /// </summary>
  public void UseOptions(MinioOptions options)
  {
    _options = options;
    _minioClient = new MinioClient()
      .WithEndpoint(_options.Endpoint)
      .WithCredentials(_options.AccessKey, _options.SecretKey)
      .WithSSL(_options.Secure)
      .Build();
  }

  /// <summary>
  /// Check if a given S3 bucket exists.
  /// </summary>
  /// <returns><c>true</c> if the bucket exists, else <c>false</c>.</returns>
  public async Task<bool> StoreExists()
  {
    var args = new BucketExistsArgs().WithBucket(_options.BucketName);
    return await _minioClient.BucketExistsAsync(args);
  }

  /// <summary>
  /// Upload a file to an S3 bucket.
  /// </summary>
  /// <param name="sourcePath">The path of the file to be uploaded.</param>
  /// <param name="targetPath">Optional Directory path to put the file in within the bucket.</param>
  /// <exception cref="BucketNotFoundException">Thrown when the given bucket doesn't exists.</exception>
  /// <exception cref="MinioException">Thrown when any other error related to MinIO occurs.</exception>
  /// <exception cref="FileNotFoundException">Thrown when the file to be uploaded does not exist.</exception>
  public async Task WriteToStore(string sourcePath, string targetPath = "")
  {
    if (!await StoreExists())
      throw new BucketNotFoundException(_options.BucketName, $"No such bucket: {_options.BucketName}");

    if (!File.Exists(sourcePath)) throw new FileNotFoundException();

    var objectName = CalculateObjectName(targetPath);
    var putObjectArgs = new PutObjectArgs()
      .WithBucket(_options.BucketName)
      .WithFileName(sourcePath)
      .WithObject(objectName);

    _logger.LogInformation("Uploading '{TargetObject} to {Bucket}...", objectName, _options.BucketName);
    await _minioClient.PutObjectAsync(putObjectArgs);
    _logger.LogInformation("Successfully uploaded {TargetObject} to {Bucket}", objectName, _options.BucketName);
  }

  public string CalculateObjectName(string sourcePath, string targetPath = "")
  {
    return Path.Combine(targetPath, Path.GetFileName(sourcePath));
  }

  /// <summary>
  /// Check if a file already exists in an S3 bucket.
  /// </summary>
  /// <param name="objectName">The name of the file to check in the bucket.</param>
  /// <exception cref="BucketNotFoundException">Thrown when the given bucket doesn't exists.</exception>
  /// <exception cref="MinioException">Thrown when any other error related to MinIO occurs.</exception>
  public async Task<bool> ResultExists(string objectName)
  {
    if (!await StoreExists())
      throw new BucketNotFoundException(_options.BucketName, $"No such bucket: {_options.BucketName}");

    var statObjectArgs = new StatObjectArgs()
      .WithBucket(_options.BucketName)
      .WithObject(objectName);

    try
    {
      _logger.LogInformation("Looking for {Object} in {Bucket}...", objectName, _options.BucketName);
      await _minioClient.StatObjectAsync(statObjectArgs);
      _logger.LogInformation("Found {Object} in {Bucket}", objectName, _options.BucketName);
      return true;
    }
    catch (ObjectNotFoundException)
    {
      _logger.LogInformation("Could not find {Object} in {Bucket}", objectName, _options.BucketName);
    }

    return false;
  }
}
