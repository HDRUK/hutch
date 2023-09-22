using System.Text.Json.Serialization;

namespace HutchAgent.Models.JobQueue;

public class JobQueueMessage
{
  [JsonPropertyName("jobId")]
  public required string JobId { get; init; }

  [JsonPropertyName("actionType")]
  public required string ActionType { get; init; }
}
