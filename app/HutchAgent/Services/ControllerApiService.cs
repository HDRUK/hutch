using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;
using HutchAgent.Config;
using HutchAgent.Constants;
using HutchAgent.Models.ControllerApi;
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
  private readonly OpenIdOptions _identityOptions;
  private readonly IFlurlClient _http;
  private readonly ControllerApiOptions _apiOptions;
  private readonly OpenIdIdentityService _identity;
  private const string _standaloneModeError = "TRE Controller API should not be used in Standalone Mode.";
  private string _accessToken = string.Empty;

  public ControllerApiService(
    IFeatureManager features,
    IFlurlClientFactory httpFactory,
    IOptions<ControllerApiOptions> apiOptions,
    IOptions<OpenIdOptions> identityOptions,
    ILogger<ControllerApiService> logger,
    OpenIdIdentityService identity)
  {
    _features = features;
    _logger = logger;
    _identity = identity;
    _identityOptions = identityOptions.Value;
    _apiOptions = apiOptions.Value;
    _http = httpFactory.Get(_apiOptions.BaseUrl);
  }

  private async Task UpdateToken()
  {
    // TODO one day support Client Credentials?
    _accessToken = await _identity.RequestUserAccessToken(
      _identityOptions.ClientId,
      _identityOptions.ClientSecret,
      _identityOptions.Username,
      _identityOptions.Password);
  }

  /// <summary>
  /// Request the details for a Bucket to upload outputs for Egress checking
  /// </summary>
  /// <param name="jobId"></param>
  /// <returns></returns>
  public async Task<MinioOptions> RequestEgressBucket(string jobId)
  {
    if (await _features.IsEnabledAsync(FeatureFlags.StandaloneMode))
      throw new InvalidOperationException(_standaloneModeError);

    var url = "Submission/GetOutputBucketInfo"
      .SetQueryParam("subId", jobId);

    _logger.LogDebug("Requesting Egress Bucket from {Url}", Url.Combine(_apiOptions.BaseUrl, url));

    if (!_identity.IsTokenValid(_accessToken)) await UpdateToken();
    return await _http.Request(url).GetAsync().ReceiveJson<MinioOptions>()
           ?? throw new InvalidOperationException(
             "No Response Body was received for an Egress Bucket request.");
    // TODO attempt refreshing if token rejected?
  }

  /// <summary>
  /// Confirm with the TRE Controller API that Egress Outputs have been transferred to the Intermediary Store.
  /// </summary>
  /// <param name="jobId">The Job Id this is for.</param>
  /// <param name="files">A list of output file object IDs in the store.</param>
  /// <exception cref="InvalidOperationException">TRE Controller API was attempted to be used in Standalone Mode.</exception>
  public async Task ConfirmOutputsTransferred(string jobId, List<string> files)
  {
    if (await _features.IsEnabledAsync(FeatureFlags.StandaloneMode))
      throw new InvalidOperationException(_standaloneModeError);

    var url = "Submission/FilesReadyForReview"
      .SetQueryParam("subId", jobId);

    _logger.LogInformation(
      "Job [{JobId}]: Confirming with TRE Controller API that Egress Outputs have been transferred", jobId);

    if (!_identity.IsTokenValid(_accessToken)) await UpdateToken();
    await _http.Request(url).PostJsonAsync(
      new FilesReadyForReviewRequest()
      {
        Files = files
      });
    // TODO attempt refreshing if token rejected?
  }

  /// <summary>
  /// Request the submission layer to update the status of a submission with the given ID.
  /// </summary>
  /// <param name="jobId">The ID of the submission to be updated.</param>
  /// <param name="status">The new status of the submission.</param>
  /// <param name="description"></param>
  /// <exception cref="InvalidOperationException"></exception>
  public async Task UpdateStatusForTre(string jobId, JobStatus status, string? description)
  {
    if (await _features.IsEnabledAsync(FeatureFlags.StandaloneMode))
      throw new InvalidOperationException(_standaloneModeError);

    var url = "Submission/UpdateStatusForTre"
      .SetQueryParams(new
      {
        subId = jobId,
        statusType = (int)status,
        description
      });

    if (!_identity.IsTokenValid(_accessToken)) await UpdateToken();
    await _http.Request(url).PostAsync();
    // TODO attempt refreshing if token rejected?
  }
}
