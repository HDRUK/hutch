using System.Text.Json;
using HutchManager.Constants;
using HutchManager.Models;
using HutchManager.Services;
using Microsoft.FeatureManagement;

namespace HutchManager.Middleware;

public class ConfigCookieMiddleware
{

  public static readonly string ConfigCookieName = ".HutchManager.Config";
  private readonly RequestDelegate _next;
  private readonly IConfiguration _config;

  public ConfigCookieMiddleware(
    IConfiguration config,
    RequestDelegate next
    )
  {
    _next = next;
    _config = config;
  }

  public async Task Invoke(HttpContext context, FeatureFlagService features)
  {
    var model = new ConfigCookieModel();
    model.Flags.Add("Registration",_config["UserAccounts:Registration"]);
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
