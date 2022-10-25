using System.Text.Json;
using System.Text.Json.Serialization;

namespace HutchManager.Dto;

public class RquestDistributionQueryTask
{
  /// <summary>
  /// The user that creates the task.
  /// </summary>
  [JsonPropertyName("owner")]
  public string Owner = String.Empty;

  /// <summary>
  /// The code for the type of distribution query.
  /// Possible values: "GENERIC", "DEMOGRAPHICS" or "ICD-MAN"
  /// </summary>
  [JsonPropertyName("code")]
  public string Code = String.Empty;

  /// <summary>
  /// The type of analysis to be carried out.
  /// Possible values: "DISTRIBUTION"
  /// </summary>
  [JsonPropertyName("analysis")]
  public string Analysis = "DISTRIBUTION";
  
  /// <summary>
  /// The unique identifier for the distribution query.
  /// </summary>
  [JsonPropertyName("uuid")]
  public string Uuid = String.Empty;
  
  /// <summary>
  /// The collection ID to run the query against.
  /// </summary>
  [JsonPropertyName("collection")]
  public string Collection = String.Empty;
}
