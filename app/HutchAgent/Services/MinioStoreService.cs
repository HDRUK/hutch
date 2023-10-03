using HutchAgent.Config;
using Microsoft.Extensions.Options;
using Minio;
using Minio.Exceptions;

namespace HutchAgent.Services;

// TODO: How to minio with oidc?
// https://min.io/docs/minio/linux/operations/external-iam/configure-openid-external-identity-management.html

public class MinioStoreService
{
  private readonly ILogger<MinioStoreService> _logger;
  private MinioClient _minioClient = null!; // indirectly init in ctor
  private MinioOptions _options = null!; // indirectly init in ctor

  public MinioStoreService(
    ILogger<MinioStoreService> logger,
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

  /// <summary>
  /// Calculate what the object name in a bucket should be based on the source file path
  /// </summary>
  /// <param name="sourcePath">Path to the file to be uploaded</param>
  /// <param name="targetPath">Optional directory path within the bucket to put the file at</param>
  /// <returns>The full object name for the file to be placed in a bucket</returns>
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

  /// <summary>
  /// For a given object in a Minio Bucket we have access to,
  /// get a direct download link.
  /// </summary>
  /// <param name="objectId">ID of the object to get a link for</param>
  /// <returns>The URL which the object can be downloaded from</returns>
  public async Task<string> GetObjectUrl(string objectId)
  {
    Stream result;

    // Check whether the object exists using statObject().
    // If the object is not found, statObject() throws an exception,
    // else it means that the object exists.
    await _minioClient.StatObjectAsync(new StatObjectArgs()
      .WithBucket(_options.BucketName)
      .WithObject(objectId));

    return await _minioClient.PresignedGetObjectAsync(new PresignedGetObjectArgs()
      .WithBucket(_options.BucketName)
      .WithObject(objectId));
  }
}
