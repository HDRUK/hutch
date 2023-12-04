using DummyControllerApi.Config;
using DummyControllerApi.Constants;
using DummyControllerApi.Models;
using DummyControllerApi.Models.WebHooks;
using Flurl.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DummyControllerApi.Services;

public class WebHookService
{
  private readonly WebHookOptions _webHookOptions;
  private readonly EgressBucketDetailsOptions _bucketDetails;

  public WebHookService(IOptions<WebHookOptions> options, IOptions<EgressBucketDetailsOptions> bucketDetails)
  {
    _bucketDetails = bucketDetails.Value;
    _webHookOptions = options.Value;
  }

  /// <summary>
  /// Send the final result to the registered Web Hook.
  /// </summary>
  /// <param name="finalOutcome">The final outcome to be sent to the Web Hook.</param>
  public async Task SendFinalOutcome(FinalOutcomeRequestModel finalOutcome)
  {
    // Check Web Hook is configured
    if (!WebHookIsConfigured(WebHookEventTypes.FinalOutcome)) return;

    // Add bucket details
    var payload = new FinalOutcomeWebHookModel()
    {
      AccessKey = _bucketDetails.AccessKey,
      Bucket = _bucketDetails.Bucket,
      File = finalOutcome.File,
      Host = _bucketDetails.Host,
      SecretKey = _bucketDetails.SecretKey,
      Secure = false,
      SubId = finalOutcome.SubId
    };

    // Send the request
    await _webHookOptions.CallbackUrl.PostJsonAsync(payload);
  }

  private bool WebHookIsConfigured(WebHookEventTypes eventType) =>
    !_webHookOptions.CallbackUrl.IsNullOrEmpty() &&
    _webHookOptions.Events.Contains(eventType.ToString(), StringComparer.OrdinalIgnoreCase);
}
