using System.Text.Json;
using HutchManager.Config;
using HutchManager.Models;
using Microsoft.Extensions.Options;

namespace HutchManager.Middleware;

public class ConfigCookieMiddleware
{

  public static readonly string ConfigCookieName = ".HutchManager.Config";
  private readonly RequestDelegate _next;
  private readonly RegistrationOptions _registrationOptions;

  public ConfigCookieMiddleware(
    IOptions<RegistrationOptions> registrationOptions,
    RequestDelegate next
    )
  {
    _next = next;
    _registrationOptions = registrationOptions.Value;
  }

  public async Task Invoke(HttpContext context)
  {
    var model = new ConfigCookieModel();
    model.Settings.Add("Registration",_registrationOptions.Registration);
    
    var jsonString = JsonSerializer.Serialize(model);
    context.Response.Cookies.Append(ConfigCookieName, jsonString);
    await _next(context);
  }
}
public static class ConfigCookieMiddlewareExtensions
{
  public static IApplicationBuilder UseConfigCookieMiddleware(
      this IApplicationBuilder builder)
  {
    return builder.UseMiddleware<ConfigCookieMiddleware>();
  }

}
