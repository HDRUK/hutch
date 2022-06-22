using System.Text.Json.Serialization;

namespace HutchManager.Dto
{
    /// <summary>
    /// Task payload returned from RQUEST Task API /query endpoint
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
