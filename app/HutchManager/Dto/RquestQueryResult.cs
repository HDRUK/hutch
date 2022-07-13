using System.Text.Json.Serialization;

namespace HutchManager.Dto
{
    /// <summary>
    /// Results payload for a Query Task.
    /// Results with no count can be used for task cancellation
    /// </summary>
    public class RquestQueryTaskResult
    {
        public RquestQueryTaskResult(string collectionId, string jobId, int? count = null)
        {
          CollectionId = collectionId;  
          JobId = jobId;
          
            if(count.HasValue)
                QueryResult = new() { Count = count.Value, Files = new List<string>() };
        }

        [JsonPropertyName("collection_id")]
        public string CollectionId { get; set; }
        
        [JsonPropertyName("uuid")]
        public string JobId { get; set; }
        
        [JsonPropertyName("status")]
        public string Status { get; set; } = "OK";
        
        [JsonPropertyName("protocolVersion")]
        public string ProtocolVersion { get; set; } = "2";

        [JsonPropertyName("queryResult")]
        public RquestQueryResult? QueryResult { get; set; }

    }

    public class RquestQueryResult
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("files")] 
        public List<string> Files { get; set; } = new List<string>();
    }
}
