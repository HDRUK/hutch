using System.Text.Json.Serialization;

namespace HutchAgent.Models;

public class FileForReviewModel
{
  [JsonPropertyName("files")] public List<string> Files { get; set; } = new();
}
