using System.Xml.Linq;
using Flurl;
using Flurl.Http;
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

  private MinioClient GetClient(MinioOptions options, string? sessionToken)
  {
    var clientBuilder = new MinioClient()
      .WithEndpoint(options.Host)
      .WithCredentials(options.AccessKey, options.SecretKey)
      .WithSSL(options.Secure);

    if (sessionToken is not null)
      clientBuilder.WithSessionToken(sessionToken);

    return clientBuilder.Build();
  }

  /// <summary>
  /// Get temporary Minio access credentials via a client access token
  /// </summary>
  /// <param name="minioBaseUrl">The base url for the minio server - i.e. a scheme (http(s)) + the configured host</param>
  /// <param name="token">The client's Access token</param>
  /// <returns>Temporary access key and secret key for use with Minio</returns>
  private async Task<(string accessKey, string secretKey, string sessionToken)> GetTemporaryCredentials(
    string minioBaseUrl, string token)
  {
    var url = minioBaseUrl
      .SetQueryParams(new
      {
        Action = "AssumeRoleWithClientGrants",
        Version = "2011-06-15", // AWS stipulates this version for this endpoint...
        DurationSeconds = 604800 // we ask for the max (7 days) - the credentials issued may be shorter
      })
      .SetQueryParam("Token", token, true);

    try
    {
      var response = await url.PostAsync().ReceiveString();

      return ParseAssumeRoleResponse(response);
    }
    catch (FlurlHttpException e)
    {
      _logger.LogError("S3 STS AssumeRole Request failed: {ResponseBody}", await e.GetResponseStringAsync());
      throw;
    }
  }

  /// <summary>
  /// Parse the XML response from an STS AssumeRole request to extract the desired details.
  /// </summary>
  /// <param name="response">The XML response from an STS AssumeRole request as a string.</param>
  /// <returns>Access Token and Secret Key from the response.</returns>
  private static (string accessKey, string secretKey, string sessionToken) ParseAssumeRoleResponse(string response)
  {
    var xml = XElement.Parse(response);
    var accessKey = xml.Descendants().Single(x => x.Name.LocalName == "AccessKeyId").Value;
    var secretKey = xml.Descendants().Single(x => x.Name.LocalName == "SecretAccessKey").Value;
    var sessionToken = xml.Descendants().Single(x => x.Name.LocalName == "SessionToken").Value;

    return (accessKey, secretKey, sessionToken);
  }

  /// <summary>
  /// Combine provided options with default fallbacks where necessary
  /// </summary>
  /// <param name="options">The provided options</param>
  /// <returns>
  /// A complete options object built from those provided,
  /// falling back on pre-configured defaults.
  /// </returns>
  private MinioOptions MergeOptionsWithDefaults(MinioOptions? options = null)
  {
    var mergedOptions = new MinioOptions
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

    return mergedOptions;
  }

  /// <summary>
  /// Create a new instance of MinioStoreService configured with the provided options.
  /// </summary>
  /// <param name="options">The provided options - omissions will fallback to Hutch's configured Default options.</param>
  /// <returns>A <see cref="MinioStoreService"/> instance configured with the provided options.</returns>
  public async Task<MinioStoreService> Create(MinioOptions? options = null)
  {
    var useOpenId = string.IsNullOrWhiteSpace(options?.SecretKey)
                    && string.IsNullOrWhiteSpace(options?.AccessKey)
                    && _identityOptions.IsConfigComplete();

    var mergedOptions = MergeOptionsWithDefaults(options);

    string? sessionToken = null;
    if (useOpenId)
    {
      _logger.LogInformation(
        "No Minio access credentials were provided directly and OIDC is configured; attempting to retrieve credentials via OIDC");

      // Get an OIDC token
      var token = await _identity.RequestClientAccessToken(_identityOptions);
      

      // Get MinIO STS credentials with the user's identity token
      // https://min.io/docs/minio/linux/developers/security-token-service/AssumeRoleWithWebIdentity.html#minio-sts-assumerolewithwebidentity
      // or with a client access token // NOTE: this seems to be unfinished; it's not in the docs site and gives 400 Bad Request on a real server
      // https://github.com/minio/minio/blob/master/docs/sts/client-grants.md
      var (accessKey, secretKey, returnedSessionToken) = await GetTemporaryCredentials(
        $"{(mergedOptions.Secure ? "https" : "http")}://{mergedOptions.Host}",
        token);

      // set the credentials to those from the STS response
      mergedOptions.AccessKey = accessKey;
      mergedOptions.SecretKey = secretKey;
      sessionToken = returnedSessionToken;
    }

    return new MinioStoreService(
      _services.GetRequiredService<ILogger<MinioStoreService>>(),
      mergedOptions,
      GetClient(mergedOptions, sessionToken));
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

    _logger.LogDebug("Uploading '{TargetObject} to {Bucket}...", objectId, _options.Bucket);
    await _minio.PutObjectAsync(putObjectArgs);
    _logger.LogDebug("Successfully uploaded {TargetObject} to {Bucket}", objectId, _options.Bucket);
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
