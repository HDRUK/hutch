using System.Text;
using System.Text.Json;
using LinkLiteManager.Constants;
using Microsoft.AspNetCore.WebUtilities;

namespace LinkLiteManager.Extensions;

public static class StringEncodingExtensions
{
  /// <summary>
  /// Encode a UTF8 string as a Base64Url string
  /// </summary>
  /// <param name="input"></param>
  public static string Utf8ToBase64Url(this string input)
      => WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(input));

  /// <summary>
  /// Serialize an object as JSON and then encode the JSON string as Base64Url
  /// </summary>
  /// <param name="input"></param>
  public static object ObjectToBase64UrlJson(this object input)
    => JsonSerializer.Serialize(input, DefaultJsonOptions.Serializer).Utf8ToBase64Url();

  /// <summary>
  /// Decode a Base64Url string to a UT8 string
  /// </summary>
  /// <param name="input"></param>
  public static string Base64UrltoUtf8(this string input)
      => Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(input));
}
