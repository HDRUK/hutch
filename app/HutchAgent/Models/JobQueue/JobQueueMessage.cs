using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace HutchAgent.Models.JobQueue;

public class JobQueueMessage
{
  [JsonPropertyName("jobId")]
  public required string JobId { get; init; }

  [JsonPropertyName("actionType")]
  public required string ActionType { get; init; }

  /// <summary>
  /// Optional payload which should be deserializable to an `{ActionType}PayloadModel` if present
  /// </summary>
  public string? Payload { get; init; }
}
