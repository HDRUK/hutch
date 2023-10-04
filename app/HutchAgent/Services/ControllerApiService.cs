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
  private readonly IFeatureManager _features;
  private readonly HttpClient _http;
  private readonly ControllerApiOptions _apiOptions;

  public ControllerApiService(
    IFeatureManager features,
    IHttpClientFactory httpFactory,
    IOptions<ControllerApiOptions> apiOptions)
  {
    _features = features;
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
    
    // TODO: make API request and convert the results into StoreOptions Model
    return new MinioOptions();
  }

  public async Task ConfirmOutputsTransferred(string jobId)
  {
    if (await _features.IsEnabledAsync(FeatureFlags.StandaloneMode))
      return;
    
    // TODO make API call
  }
}
