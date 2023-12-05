using Flurl.Http.Configuration;

namespace DummyControllerApi.Utilities;

public class UntrustedCertClientFactory : DefaultHttpClientFactory
{
  public override HttpMessageHandler CreateMessageHandler()
  {
    return new HttpClientHandler
    {
      ServerCertificateCustomValidationCallback = (_, _, _, _) => true
    };
  }
}
