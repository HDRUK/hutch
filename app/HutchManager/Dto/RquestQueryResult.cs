using System.Text.Json.Serialization;
using Newtonsoft.Json;

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

        [JsonProperty(PropertyName="collection_id")]
        public string CollectionId { get; set; }
        
        [JsonProperty(PropertyName="uuid")]
        public string JobId { get; set; }
        
        [JsonProperty(PropertyName="status")]
        public string Status { get; set; } = "OK";
        
        [JsonProperty(PropertyName="protocolVersion")]
        public string ProtocolVersion { get; set; } = "2";

        [JsonProperty(PropertyName="queryResult")]
        public RquestQueryResult? QueryResult { get; set; }

    }

    public class RquestQueryResult
    {
        [JsonProperty(PropertyName="count")]
        public int Count { get; set; }

        [JsonProperty(PropertyName="files")] 
        public List<string> Files { get; set; } = new List<string>();
    }
}
