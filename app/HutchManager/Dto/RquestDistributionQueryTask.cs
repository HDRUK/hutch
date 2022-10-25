using System.Text.Json;
using System.Text.Json.Serialization;

namespace HutchManager.Dto;

public class RquestDistributionQueryTask
{
  /// <summary>
  /// The user that creates the task.
  /// </summary>
  [JsonPropertyName("owner")]
  public string Owner { get; set; } = String.Empty;

  /// <summary>
  /// The code for the type of distribution query.
  /// Possible values: "GENERIC", "DEMOGRAPHICS" or "ICD-MAN"
  /// </summary>
  [JsonPropertyName("code")]
  public string Code  { get; set; } = String.Empty;

  /// <summary>
  /// The type of analysis to be carried out.
  /// Possible values: "DISTRIBUTION"
  /// </summary>
  [JsonPropertyName("analysis")]
  public string Analysis { get; set; } = "DISTRIBUTION";

  /// <summary>
  /// The unique identifier for the distribution query.
  /// </summary>
  [JsonPropertyName("uuid")]
  public string Uuid { get; set; } = String.Empty;

  /// <summary>
  /// The collection ID to run the query against.
  /// </summary>
  [JsonPropertyName("collection")]
  public string Collection { get; set; } = String.Empty;
}
