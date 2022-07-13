using System.Text.Json.Serialization;

namespace HutchManager.Dto
{
    /// <summary>
    /// Task payload returned from RQUEST Task API /query endpoint
    /// </summary>
    public class RquestQueryTask
    {
        [JsonPropertyName("uuid")]
        public string Uuid
        { 
          set { JobId = value; }
        }

        [JsonPropertyName("job_id")] 
        public string JobId { get; set; } = string.Empty;

        [JsonPropertyName("cohort")]
        public RquestQuery Query { get; set; } = new();
        
        [JsonPropertyName("activity_source_id")] 
        public int ActivitySourceId { get; set; }
    }
}
