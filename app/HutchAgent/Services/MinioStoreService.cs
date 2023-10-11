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
  private readonly ILogger<MinioStoreServiceFactory> _logger;
  private readonly OpenIdIdentityService _identity;
  private readonly OpenIdOptions _identityOptions;

  public MinioStoreServiceFactory(
    IOptions<MinioOptions> defaultOptions,
    IServiceProvider services,
    IOptions<OpenIdOptions> identityOptions,
    ILogger<MinioStoreServiceFactory> logger,
    OpenIdIdentityService identity)
  {
    DefaultOptions = defaultOptions.Value;
    _services = services;
    _logger = logger;
    _identity = identity;
    _identityOptions = identityOptions.Value;
  }

  /// <summary>
  /// The Default Options for a Minio Store, as configured.
  /// </summary>
  public MinioOptions DefaultOptions { get; }

  private MinioClient GetClient(MinioOptions options)
  {
    return new MinioClient()
      .WithEndpoint(options.Host)
      //.WithSessionToken() // Might we also need this for STS?
      .WithCredentials(options.AccessKey, options.SecretKey)
      .WithSSL(options.Secure)
      .Build();
  }

  /// <summary>
  /// Combine provided options with default fallbacks where necessary,
  /// and optionally fetching missing credentials via OIDC if configured
  /// </summary>
  /// <param name="options">The provided options</param>
  /// <returns>
  /// A complete options object built from those provided,
  /// falling back on OIDC for credentials if possible,
  /// and pre-configured defaults for everything else.
  /// </returns>
  private async Task<MinioOptions> MergeOptions(MinioOptions? options = null)
  {
    var useOpenId = string.IsNullOrWhiteSpace(options?.SecretKey)
                    && string.IsNullOrWhiteSpace(options?.AccessKey)
                    && _identityOptions.IsConfigComplete();

    if (useOpenId)
    {
      _logger.LogInformation(
        "No Minio access credentials were provided directly and OIDC is configured; attempting to retrieve credentials via OIDC");

      // Get an OIDC Access token
      var token = await _identity.RequestUserAccessToken(_identityOptions);

      // Get a MinIO STS with the access token
      // https://min.io/docs/minio/linux/developers/security-token-service/AssumeRoleWithWebIdentity.html#minio-sts-assumerolewithwebidentity
      // looks like an XML response? :( // TODO Test in postman?

      // set the credentials to those from the STS response

      // TODO do we need the session token? per the docs, "some clients" do...
    }

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
  public async Task<MinioStoreService> Create(MinioOptions? options = null)
  {
    var mergedOptions = await MergeOptions(options);

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
  private readonly IMinioClient _minio;

  public MinioStoreService(
    ILogger<MinioStoreService> logger,
    MinioOptions options,
    IMinioClient minio)
  {
    _logger = logger;
    _options = options;
    _minio = minio;
    
    // TODO One day we might need to handle expiry / refresh of credentials
    // since the options provided to this instance may contain temporary credentials.
    // Fetching new ones within the lifetime of a single store instance?
    // Currently we hope instances are shortlived enough (due to use of the factory)
    // and credentials are longlived enough that it won't matter,
    // since we get new instances from the factory for distinct job actions
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
  /// <param name="objectId">Intended ObjectId in the target storage.</param>
  /// <exception cref="BucketNotFoundException">Thrown when the given bucket doesn't exists.</exception>
  /// <exception cref="MinioException">Thrown when any other error related to MinIO occurs.</exception>
  /// <exception cref="FileNotFoundException">Thrown when the file to be uploaded does not exist.</exception>
  public async Task WriteToStore(string sourcePath, string objectId)
  {
    if (!await StoreExists())
      throw new BucketNotFoundException(_options.Bucket, $"No such bucket: {_options.Bucket}");

    if (!File.Exists(sourcePath)) throw new FileNotFoundException();
    var putObjectArgs = new PutObjectArgs()
      .WithBucket(_options.Bucket)
      .WithFileName(sourcePath)
      .WithObject(objectId);

    _logger.LogInformation("Uploading '{TargetObject} to {Bucket}...", objectId, _options.Bucket);
    await _minio.PutObjectAsync(putObjectArgs);
    _logger.LogInformation("Successfully uploaded {TargetObject} to {Bucket}", objectId, _options.Bucket);
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

  /// <summary>
  /// Upload the contents of a Directory and its subdirectories,
  /// optionally with an objectId prefix to "subdirectory" the objects in the target bucket.
  /// </summary>
  /// <param name="sourcePath">The starting directory path. Must be a directory not a single file.</param>
  /// <param name="targetPrefix">Optional prefix to prepend to any uploaded objects (serves as a target directory path within the target bucket).</param>
  /// <returns>A List of object IDs uploaded (i.e. effective file paths relative to the bucket root).</returns>
  public async Task<List<string>> UploadFilesRecursively(string sourcePath, string targetPrefix = "")
  {
    var a = File.GetAttributes(sourcePath);
    if ((a & FileAttributes.Directory) != FileAttributes.Directory)
      throw new ArgumentException(
        $"Expected a path to a Directory, but got a file: {sourcePath}", nameof(sourcePath));

    return await UploadFilesRecursively(sourcePath, "", targetPrefix);
  }

  private async Task<List<string>> UploadFilesRecursively(string sourceRoot, string sourceSubPath, string targetPrefix)
  {
    // We do a bunch of path shenanigans to ensure relative directory paths are maintained inside the bucket
    var sourcePath = Path.Combine(sourceRoot, sourceSubPath);
    var a = File.GetAttributes(sourcePath);

    // Directory
    if ((a & FileAttributes.Directory) == FileAttributes.Directory)
    {
      var results = new List<string>();
      foreach (var entry in Directory.EnumerateFileSystemEntries(sourcePath))
      {
        if (!Path.EndsInDirectorySeparator(sourceRoot))
          sourceRoot += Path.DirectorySeparatorChar;
        var relativeSubPath = entry.Replace(sourceRoot, "");

        results.AddRange(await UploadFilesRecursively(sourceRoot, relativeSubPath, targetPrefix));
      }

      return results;
    }

    // Single File
    var objectId = Path.Combine(targetPrefix, sourceSubPath);
    await WriteToStore(sourcePath, objectId);
    return new() { objectId };
  }
}
