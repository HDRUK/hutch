using System.Globalization;
using Flurl;
using LinkLiteManager.Auth;

namespace LinkLiteManager.Extensions;

public static class HttpRequestExtensions
{
  public static CultureInfo GetUICulture(this HttpRequest request)
  {
    CultureInfo culture = CultureInfo.CurrentUICulture;

    try
    {
      var requestCultureName = 
        // Try the User first
        request.HttpContext.User.FindFirst(CustomClaimTypes.UICulture)?.Value
        // Else use the Header from the frontend
        ?? request.Headers.AcceptLanguage.FirstOrDefault();

      if (requestCultureName is not null)
        culture = CultureInfo.GetCultureInfoByIetfLanguageTag(requestCultureName);
    }
    catch (CultureNotFoundException)
    {
      // No worries, we'll just continue with CurrentUICulture
    }

    return culture;
  }


  // These don't strictly extend HttpRequest but do require it to work :)

  public static Uri ToLocalUrl(this string path, HttpRequest request)
      => Url.Parse(Url.Combine(
            $"{request.Scheme}://{request.Host}",
            path))
        .SetQueryParam("lng", request.GetUICulture().Name)
        .ToUri();

  public static string ToLocalUrlString(this string path, HttpRequest request)
      => path.ToLocalUrl(request).ToString();
}
