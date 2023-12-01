using DummyControllerApi.Config;
using Microsoft.Extensions.Options;

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
  public async Task SendFinalOutcome()
  {
  }
}
