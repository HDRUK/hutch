using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace LinkLiteManager.Constants;

public static class DefaultJsonOptions
{
  public static Action<JsonOptions> Configure = o =>
  {
    o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
  };

  public static JsonSerializerOptions Serializer
  {
    get
    {
      JsonOptions o = new();
      Configure(o);
      return o.JsonSerializerOptions;
    }
  }
}
