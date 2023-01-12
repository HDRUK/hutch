using System.Text.Json.Serialization;

namespace HutchManager.Dto;

/// <summary>
/// Results payload for a Query Task.
/// Results with no count can be used for task cancellation
/// </summary>
public class RquestAvailabilityResult
{
  [JsonPropertyName("collection_id")]
  public string CollectionId { get; set; } = string.Empty;

  [JsonPropertyName("status")]
  public string Status { get; set; } = string.Empty;
        
  [JsonPropertyName("protocol_version")]
  public string ProtocolVersion { get; set; } = "2";

  [JsonPropertyName("query_result")]
  public AvailabilityCount QueryResult { get; set; } = new();
}

public class AvailabilityCount
{
  [JsonPropertyName("count")]
  public int Count { get; set; } = 0;
}
