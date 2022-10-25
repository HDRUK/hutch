using System.Text.Json.Serialization;

namespace HutchManager.Dto
{
    /// <summary>
    /// Task payload returned from RQUEST Task API /query endpoint
    /// </summary>
    public class RquestQueryTask : IRquestTask
    {   /// <summary>
        /// UUid sets JobId using "uuid" as the property name
        /// This is due to the incoming RQuest query using "uuid" as the key name.
        /// </summary>
        [JsonPropertyName("uuid")]
        public string Uuid
        { 
          set { JobId = value; }
        }
        /// <summary>
        /// JobId is set to property name "job_id" when RquestQueryTask is serialized
        /// This is so the representation of JobId is consistent between Agent and Manager
        /// </summary>
        [JsonPropertyName("job_id")] 
        public string JobId { get; set; } = string.Empty;

        [JsonPropertyName("cohort")]
        public RquestQuery Query { get; set; } = new();
        
        [JsonPropertyName("activity_source_id")] 
        public int ActivitySourceId { get; set; }
    }
}
