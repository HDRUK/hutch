using System.Text.Json.Serialization;
using HutchAgent.Constants;

namespace HutchAgent.Models;

public class ApprovalResult
{
  [JsonPropertyName("status")] public ApprovalType Status { get; set; }

  [JsonPropertyName("fileResults")] public Dictionary<string, bool> FileResults { get; set; } = new();
}
