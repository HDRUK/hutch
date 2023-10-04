using HutchAgent.Config;
using HutchAgent.Constants;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;

namespace HutchAgent.Services;

/// <summary>
/// Contains all Interactions with the TRE Controller API
/// </summary>
public class ControllerApiService
{
  private readonly ILogger<ControllerApiService> _logger;
  private readonly IFeatureManager _features;
  private readonly HttpClient _http;
  private readonly ControllerApiOptions _apiOptions;
  private readonly string _bucketRequestPath = "api/Submission/GetOutputBucketInfo/?subId={0}";

  public ControllerApiService(
    IFeatureManager features,
    IHttpClientFactory httpFactory,
    IOptions<ControllerApiOptions> apiOptions,
    ILogger<ControllerApiService> logger)
  {
    _features = features;
    _logger = logger;
    _apiOptions = apiOptions.Value;
    _http = httpFactory.CreateClient();
    _http.BaseAddress = new Uri(_apiOptions.BaseUrl);
  }

  /// <summary>
  /// Request the details for a Bucket to upload outputs for Egress checking
  /// </summary>
  /// <param name="jobId"></param>
  /// <returns></returns>
  public async Task<MinioOptions> RequestEgressBucket(string jobId)
  {
    if (await _features.IsEnabledAsync(FeatureFlags.StandaloneMode))
      throw new InvalidOperationException("TRE Controller API should not be used in Standalone Mode.");

    // Combine URIs
    var baseUri = new Uri(_apiOptions.BaseUrl);
    var fullUri = new Uri(baseUri, string.Format(_bucketRequestPath, jobId));

    // Request the MinIO bucket details.
    try
    {
      var response = await _http.GetAsync(fullUri);

      // If the request was successful, deserialise the options and return them.
      var body = await response.Content.ReadFromJsonAsync<MinioOptions>();

      return body ?? throw new Exception();
    }
    catch (Exception)
    {
      _logger.LogError("Unable to fetch egress bucket details from {Url}", fullUri);
      throw;
    }
  }

  public async Task ConfirmOutputsTransferred(string jobId)
  {
    if (await _features.IsEnabledAsync(FeatureFlags.StandaloneMode))
      return;

    // TODO make API call
  }

  /// <summary>
  /// Get the list of files associated with a job ID that are ready to be reviewed.
  /// </summary>
  /// <param name="jobId"></param>
  /// <returns></returns>
  public async Task<List<string>> GetFilesReadyForReview(string jobId)
  {
    return new List<string>();
  }

  public async Task AddNewDataEgress()
  {
  }

  public async Task Approval()
  {
  }
}
