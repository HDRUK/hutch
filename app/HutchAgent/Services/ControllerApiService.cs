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
  private readonly IFlurlClient _http;
  private readonly ControllerApiOptions _apiOptions;

  public ControllerApiService(
    IFeatureManager features,
    IFlurlClientFactory httpFactory,
    IOptions<ControllerApiOptions> apiOptions,
    ILogger<ControllerApiService> logger)
  {
    _features = features;
    _logger = logger;
    _apiOptions = apiOptions.Value;
    _http = httpFactory.Get(_apiOptions.BaseUrl); // TODO what if standalone mode?
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

    var url = "Submission/GetOutputBucketInfo"
      .SetQueryParam("subId", jobId);

    _logger.LogDebug("Requesting Egress Bucket from {Url}", Url.Combine(_apiOptions.BaseUrl, url));

    return await _http.Request(url).GetAsync().ReceiveJson<MinioOptions>()
           ?? throw new InvalidOperationException(
             "No Response Body was received for an Egress Bucket request.");
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
      throw new InvalidOperationException("TRE Controller API should not be used in Standalone Mode.");

    var url = "Submission/FilesReadyForReview"
      .SetQueryParam("subId", jobId);

    _logger.LogInformation(
      "Job [{JobId}]: Confirming with TRE Controller API that Egress Outputs have been transferred", jobId);

    await _http.Request(url).PostJsonAsync(
      new FilesReadyForReviewRequest()
      {
        Files = files
      });
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
      throw new InvalidOperationException("TRE Controller API should not be used in Standalone Mode.");

    // Combine URIs
    var fullUri = new UriBuilder(_updateStatusPath);

    // add query params
    var query = HttpUtility.ParseQueryString(fullUri.Query);
    query.Set("subId", jobId);
    query.Set("statusType", status.ToString());
    query.Set("description", description);

    // send the update
    try
    {
      await _http.PostAsync(fullUri.Uri, null);
    }
    catch (Exception e)
    {
      _logger.LogError(exception: e, "Request to update status for {JobId} failed", jobId);
      throw;
    }
  }
}
