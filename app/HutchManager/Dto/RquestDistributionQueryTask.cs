using System.Text.Json;
using System.Text.Json.Serialization;

namespace HutchManager.Dto;

public class RquestDistributionQueryTask : IRquestTask
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
  /// UUid sets JobId using "uuid" as the property name
  /// This is due to the incoming RQuest query using "uuid" as the key name.
  /// </summary>
  [JsonPropertyName("uuid")]
  public string Uuid
  { 
    set { JobId = value; }
  }

  /// <summary>
  /// The collection ID to run the query against.
  /// </summary>
  [JsonPropertyName("collection")]
  public string Collection { get; set; } = String.Empty;

  /// <summary>
  /// JobId is set to property name "job_id" when RquestQueryTask is serialized
  /// This is so the representation of JobId is consistent between Agent and Manager
  /// </summary>
  [JsonPropertyName("job_id")] 
  public string JobId { get; set; } = string.Empty;
  
  [JsonPropertyName("activity_source_id")]
  public int ActivitySourceId { get; set; }
}
