using System.Text.Json.Serialization;

namespace HutchManager.Dto
{
  public class QueryResult
  {
    public string Status { get; set; } = "OK";
    public string ProtocolVersion { get; set; } = "2";

    [JsonPropertyName("activity_source_id")]
    public int ActivitySourceId { get; set; }

    [JsonPropertyName("job_id")] 
    public string JobId { get; set; } = string.Empty;

    [JsonPropertyName("queryResult")]
    public QueryResultCount Results { get; set; } = new();
  }

  public class QueryResultCount
  {
    public int? Count { get; set; } = null;
    public List<string> Files { get; set; } = new List<string>();
  }
}

