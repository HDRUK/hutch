using System.Text.Json.Serialization;
using Org.BouncyCastle.Utilities.Collections;

namespace HutchManager.Dto
{
  /// <summary>
  /// Task payload returned from RQUEST Task API /query endpoint
  /// </summary>
  public class QueryResult
  {
    public QueryResult(string jobId, int? count = null)
    {
      JobId = jobId;

      if(count.HasValue)
        Result = new() { Count = count.Value };
    }
    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;

    [JsonPropertyName("protocol_version")] public string ProtocolVersion { get; set; } = string.Empty;

    [JsonPropertyName("activity_source_id")]
    public int ActivitySourceId { get; set; }

    [JsonPropertyName("job_id")] public string JobId { get; set; }
    
    [JsonPropertyName("query_result")]
    public QueryResultCount? Result { get; set; }
    
  }
  
  public class QueryResultCount
  {
    [JsonPropertyName("count")]
    public int Count { get; set; }
  }
}
