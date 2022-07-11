using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace HutchManager.Dto
{
  /// <summary>
  /// Task payload returned from RQUEST Task API /query endpoint
  /// </summary>
  public class QueryResult
  {
    public QueryResult(int activity_source_id, string job_id, int? count=null)
    {
      ActivitySourceId = activity_source_id;
      JobId = job_id;
      
      if(count.HasValue)
        Results = new() { Count = count.Value };
    }

    [JsonProperty(PropertyName = "status")]
    public string Status { get; set; } = "OK";

    [JsonProperty(PropertyName = "protocolVersion")]
    public string ProtocolVersion { get; set; } = "2";

    [JsonProperty(PropertyName="activity_source_id")]
    public int ActivitySourceId { get; set; }

    [JsonProperty(PropertyName="job_id")] public string JobId { get; set; }
    
    [JsonProperty(PropertyName="query_result")]
    public QueryResultCount Results { get; set; }
    
  }
  
  public class QueryResultCount
  {
    [JsonProperty(PropertyName="count")]
    public int Count { get; set; }

  }
}
