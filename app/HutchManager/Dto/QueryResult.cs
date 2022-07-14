using System.Text.Json.Serialization;

namespace HutchManager.Dto
{
  public class QueryResult
  {
    public string Status { get; set; } = "OK";
    public string ProtocolVersion { get; set; } = "2";

    [JsonPropertyName("activity_source_id")]
    public int ActivitySourceId { get; set; }

    [JsonPropertyName("job_id")] public string JobId { get; set; } = String.Empty;

    [JsonPropertyName("queryResult")]
    public QueryResultCount Results
    {
      get { return _result; }
      set
      {
        _result = value;
        _count = value.Count;
      }
    }

    private QueryResultCount _result= new QueryResultCount();
    internal int? _count;

  }

  public class QueryResultCount
  {
    public int? Count { get; set; } = null;
    public List<string> Files { get; set; } = new List<string>();
  }
}

