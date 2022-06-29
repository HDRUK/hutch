using System.Text.Json.Serialization;

namespace HutchManager.Dto
{
    /// <summary>
    /// Results payload for a Query Task.
    /// Results with no count can be used for task cancellation
    /// </summary>
    public class RquestQueryTaskResult
    {
        public RquestQueryTaskResult(string jobId, int? count = null)
        {
            JobId = jobId;

            if(count.HasValue)
                QueryResult = new() { Count = count.Value };
        }

        [JsonPropertyName("uuid")]
        public string JobId { get; set; }

        [JsonPropertyName("query_result")]
        public RquestQueryResult? QueryResult { get; set; }
    }

    public class RquestQueryResult
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}
