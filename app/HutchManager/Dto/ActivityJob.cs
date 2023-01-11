using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace HutchManager.Dto;

/// <summary>
/// Represents an activity as its type and properties as its payload.
/// </summary>
public class ActivityJob
{
  /// <summary>
  /// The type of job. e.g. Availability Query.
  /// </summary>
  [JsonPropertyName("type")]
  public string Type { get; set; } = string.Empty;
  
  /// <summary>
  /// UUID identifying the job.
  /// </summary>
  [JsonPropertyName("job_id")]
  public string JobId { get; set; } = string.Empty;
  
  /// <summary>
  /// The ID of the activity source.
  /// </summary>
  [JsonPropertyName("activity_source_id")] 
  public int ActivitySourceId { get; set; }
  
  /// <summary>
  /// The properties of the job.
  /// </summary>
  [JsonPropertyName("payload")]
  public JsonElement Payload { get; set; }
}
