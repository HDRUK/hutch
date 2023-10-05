using HutchAgent.Config;
using Microsoft.Extensions.Options;
using Minio;
using Minio.Exceptions;

namespace HutchAgent.Services;

// TODO: How to minio with oidc?
// https://min.io/docs/minio/linux/operations/external-iam/configure-openid-external-identity-management.html

public class MinioStoreServiceFactory
{
  private readonly IServiceProvider _services;

  public MinioStoreServiceFactory(IOptions<MinioOptions> defaultOptions, IServiceProvider services)
  {
    DefaultOptions = defaultOptions.Value;
    _services = services;
  }

  /// <summary>
  /// The Default Options for a Minio Store, as configured.
  /// </summary>
  public MinioOptions DefaultOptions { get; }

  private MinioClient GetClient(MinioOptions options)
  {
    return new MinioClient()
      .WithEndpoint(options.Host)
      .WithCredentials(options.AccessKey, options.SecretKey)
      .WithSSL(options.Secure)
      .Build();
  }

  private MinioOptions MergeOptions(MinioOptions? options = null)
  {
    // TODO OIDC Minio Credentials support

    return new()
    {
      Host = string.IsNullOrWhiteSpace(options?.Host)
        ? DefaultOptions.Host
        : options.Host,
      AccessKey = string.IsNullOrWhiteSpace(options?.AccessKey)
        ? DefaultOptions.AccessKey
        : options.AccessKey,
      SecretKey = string.IsNullOrWhiteSpace(options?.SecretKey)
        ? DefaultOptions.SecretKey
        : options.SecretKey,
      Secure = options?.Secure ?? DefaultOptions.Secure,
      Bucket = string.IsNullOrWhiteSpace(options?.Bucket)
        ? DefaultOptions.Bucket
        : options.Bucket,
    };
  }

  /// <summary>
  /// Create a new instance of MinioStoreService configured with the provided options.
  /// </summary>
  /// <param name="options">The provided options - omissions will fallback to Hutch's configured Default options.</param>
  /// <returns>A <see cref="MinioStoreService"/> instance configured with the provided options.</returns>
  public MinioStoreService Create(MinioOptions? options = null)
  {
    var mergedOptions = MergeOptions(options);

    return new MinioStoreService(
      _services.GetRequiredService<ILogger<MinioStoreService>>(),
      mergedOptions,
      GetClient(mergedOptions));
  }
}

public class MinioStoreService
{
  private readonly ILogger<MinioStoreService> _logger;
  private readonly MinioOptions _options;
  private readonly MinioClient _minio;

  public MinioStoreService(
    ILogger<MinioStoreService> logger,
    MinioOptions options,
    MinioClient minio)
  {
    _logger = logger;
    _options = options;
    _minio = minio;
  }

  /// <summary>
  /// Check if a given S3 bucket exists.
  /// </summary>
  /// <returns><c>true</c> if the bucket exists, else <c>false</c>.</returns>
  public async Task<bool> StoreExists()
  {
    var args = new BucketExistsArgs().WithBucket(_options.Bucket);
    return await _minio.BucketExistsAsync(args);
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
      throw new BucketNotFoundException(_options.Bucket, $"No such bucket: {_options.Bucket}");

    if (!File.Exists(sourcePath)) throw new FileNotFoundException();

    var objectName = CalculateObjectName(targetPath);
    var putObjectArgs = new PutObjectArgs()
      .WithBucket(_options.Bucket)
      .WithFileName(sourcePath)
      .WithObject(objectName);

    _logger.LogInformation("Uploading '{TargetObject} to {Bucket}...", objectName, _options.Bucket);
    await _minio.PutObjectAsync(putObjectArgs);
    _logger.LogInformation("Successfully uploaded {TargetObject} to {Bucket}", objectName, _options.Bucket);
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
      throw new BucketNotFoundException(_options.Bucket, $"No such bucket: {_options.Bucket}");

    var statObjectArgs = new StatObjectArgs()
      .WithBucket(_options.Bucket)
      .WithObject(objectName);

    try
    {
      _logger.LogInformation("Looking for {Object} in {Bucket}...", objectName, _options.Bucket);
      await _minio.StatObjectAsync(statObjectArgs);
      _logger.LogInformation("Found {Object} in {Bucket}", objectName, _options.Bucket);
      return true;
    }
    catch (ObjectNotFoundException)
    {
      _logger.LogInformation("Could not find {Object} in {Bucket}", objectName, _options.Bucket);
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
    // Check whether the object exists using statObject().
    // If the object is not found, statObject() throws an exception,
    // else it means that the object exists.
    await _minio.StatObjectAsync(new StatObjectArgs()
      .WithBucket(_options.Bucket)
      .WithObject(objectId));

    return await _minio.PresignedGetObjectAsync(new PresignedGetObjectArgs()
      .WithExpiry((int)TimeSpan.FromDays(1).TotalSeconds)
      .WithBucket(_options.Bucket)
      .WithObject(objectId));
  }
}
