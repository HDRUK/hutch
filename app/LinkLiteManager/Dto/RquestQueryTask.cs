using System.Text.Json.Serialization;

namespace LinkLiteManager.Dto
{
    /// <summary>
    /// Task payload returned from RQUEST Connector API /query endpoint
    /// </summary>
    public class RquestQueryTask
    {
        [JsonPropertyName("task_id")]
        public string TaskId { get; set; } = string.Empty;

        [JsonPropertyName("cohort")]
        public RquestQuery Query { get; set; } = new();

        [JsonPropertyName("collection_id")] public string CollectionId { get; set; } = string.Empty;
    }
}
