using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace HutchAgent.Models.JobQueue;

// for some unknown reason, these *need* JsonPropertyName attributes to work
// when serializing/deserializing with the queue?
public class JobQueueMessage
{
  [JsonPropertyName("jobId")]
  public required string JobId { get; init; }

  [JsonPropertyName("actionType")]
  public required string ActionType { get; init; }

  /// <summary>
  /// Optional payload which should be deserializable to an `{ActionType}PayloadModel` if present
  /// </summary>
  [JsonPropertyName("payload")]
  public string? Payload { get; set; }
}
