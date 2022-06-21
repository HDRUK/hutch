using System.Text.Json;
using HutchManager.Models;
using HutchManager.Services;

namespace HutchManager.Middleware;

public class ConfigCookieMiddleware
{

  public static readonly string ConfigCookieName = ".HutchManager.Config";

  private readonly RequestDelegate _next;

  public ConfigCookieMiddleware(
    RequestDelegate next
    )
  {
    _next = next;
  }

  public async Task Invoke(HttpContext context, FeatureFlagService features)
  {
    var model = new ConfigCookieModel();
    model.Flags = await features.List();
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
