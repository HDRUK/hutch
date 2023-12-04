using System.Text.Json;
using DummyControllerApi.Config;
using DummyControllerApi.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DummyControllerApi.Services;

public class WebHookService
{
  private readonly WebHookOptions _options;

  public WebHookService(IOptions<WebHookOptions> options)
  {
    _options = options.Value;
  }

  /// <summary>
  /// Send the final result to the registered Web Hook.
  /// </summary>
  /// <param name="finalOutcome">The final outcome to be sent to the Web Hook.</param>
  public async Task SendFinalOutcome(FinalOutcomeRequestModel finalOutcome)
  {
    // Check Web Hook is configured
    if (!WebHookIsConfigured) return;

    // Serialise the outcome
    var serialisedOutcome = JsonSerializer.Serialize(finalOutcome);
    var content = new StringContent(serialisedOutcome);

    // Todo: 1. check for verify SSL; 2. check event list

    // Send the request
    using var client = new HttpClient();
    await client.PostAsync(_options.CallbackUrl, content);
  }

  private bool WebHookIsConfigured => !_options.CallbackUrl.IsNullOrEmpty();
}
