using HutchAgent.Config;
using Microsoft.Extensions.Options;
using Minio;
using Minio.Exceptions;

namespace HutchAgent.Services;

public class MinioService : IResultsStoreWriter
{
  private readonly MinioClient _minioClient;
  private readonly ILogger<MinioService> _logger;
  private readonly MinioOptions _options;

  public MinioService(ILogger<MinioService> logger, IOptions<MinioOptions> options)
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
  /// <param name="resultPath">The path of the file to be uploaded.</param>
  /// <exception cref="BucketNotFoundException">Thrown when the given bucket doesn't exists.</exception>
  /// <exception cref="MinioException">Thrown when any other error related to MinIO occurs.</exception>
  /// <exception cref="FileNotFoundException">Thrown when the file to be uploaded does not exist.</exception>
  public async Task WriteToStore(string resultPath)
  {
    if (!await StoreExists())
      throw new BucketNotFoundException(_options.BucketName, $"No such bucket: {_options.BucketName}");

    if (!File.Exists(resultPath)) throw new FileNotFoundException();

    var objectName = Path.GetFileName(resultPath);
    var putObjectArgs = new PutObjectArgs()
      .WithBucket(_options.BucketName)
      .WithFileName(resultPath)
      .WithObject(objectName);

    _logger.LogInformation($"Uploading {objectName} to {_options.BucketName}...");
    await _minioClient.PutObjectAsync(putObjectArgs);
    _logger.LogInformation($"Successfully uploaded {objectName} to {_options.BucketName}.");
  }

  /// <summary>
  /// Check if a file already exists in an S3 bucket.
  /// </summary>
  /// <param name="resultPath">The name of the file to check in the bucket.</param>
  /// <exception cref="BucketNotFoundException">Thrown when the given bucket doesn't exists.</exception>
  /// <exception cref="MinioException">Thrown when any other error related to MinIO occurs.</exception>
  public async Task<bool> ResultExists(string resultPath)
  {
    var resultName = Path.GetFileName(resultPath);
    if (!await StoreExists())
      throw new BucketNotFoundException(_options.BucketName, $"No such bucket: {_options.BucketName}");

    var statObjectArgs = new StatObjectArgs()
      .WithBucket(_options.BucketName)
      .WithObject(resultName);

    try
    {
      _logger.LogInformation($"Looking for {resultName} in {_options.BucketName}...");
      await _minioClient.StatObjectAsync(statObjectArgs);
      _logger.LogInformation($"Found {resultName} in {_options.BucketName}.");
      return true;
    }
    catch (ObjectNotFoundException)
    {
      _logger.LogInformation($"Could not find {resultName} in {_options.BucketName}.");
    }

    return false;
  }
}
