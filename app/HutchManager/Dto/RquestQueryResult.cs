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

    /// <summary>
    /// Representation of the JSON of a Distribution Query results object in
    /// RQuest format.
    /// </summary>
    public class DistributionQueryTaskResult
    {
      public DistributionQueryTaskResult(string collectionId, string jobId)
      {
        CollectionId = collectionId;
        JobId = jobId;
      }

      /// <summary>
      /// The unique ID for the collection the query is run against.
      /// </summary>
      [JsonPropertyName("collection_id")]
      public string CollectionId { get; set; }

      [JsonPropertyName("message")] 
      public string? Message { get; set; } = null;

      [JsonPropertyName("protocolVersion")] 
      public string ProtocolVersion { get; set; } = "V2";

      /// <summary>
      /// JSON object containing the query results.
      /// </summary>
      [JsonPropertyName("queryResult")] 
      public DistributionQueryResult QueryResult { get; set; } = new();

      [JsonPropertyName("status")] public string Status { get; set; } = "OK";

      /// <summary>
      /// The unique ID of the job being run.
      /// </summary>
      [JsonPropertyName("uuid")]
      public string JobId { get; set; }
    }

    public class DistributionQueryResult
    {
      /// <summary>
      /// This is the total count of subjects from this collection as a result of the query,
      /// or empty value if number of hits is under the Collection security threshold.
      /// </summary>
      [JsonPropertyName("count")]
      public int? Count { get; set; }
      
      [JsonPropertyName("datasetsCount")]
      public int DatasetsCount { get; set; }

      /// <summary>
      /// Array of JSON objects representing the files uploaded for
      /// Distribution Queries.
      /// </summary>
      [JsonPropertyName("files")] 
      public List<FileObject> Files { get; set; } = new();
    }

    public class FileObject
    {
      /// <summary>
      /// A base-64 encoded string containing TAB separated data.
      /// </summary>
      [JsonPropertyName("file_data")] 
      public string Data { get; set; } = string.Empty;
      
      [JsonPropertyName("file_description")]
      public string? Description { get; set; }
      
      /// <summary>
      /// The name of the file.
      /// Can be one of the follwoing:
      ///   "code.distribution"
      ///   "demographics.distribution"
      ///   "icd_level1.distribution"
      ///   "icd_level2.distribution"
      /// </summary>
      [JsonPropertyName("file_name")]
      public string Name { get; set; } = String.Empty;

      /// <summary>
      /// The number of KB that make up the file.
      /// </summary>
      [JsonPropertyName("file_size")] 
      public double Size { get; set; } = 0;

      [JsonPropertyName("file_type")] 
      public string Type = "BCOS";

      [JsonPropertyName("file_sensitive")] 
      public bool Sensitive { get; set; } = false;

      /// <summary>
      /// The attribute contains the path to the result file on REST API server.
      /// The param contains value when there is sensitive data that cannot leave the safe node machine
      /// or when the result file is too large.
      /// </summary>
      [JsonPropertyName("file_reference")] 
      public string Reference { get; set; } = string.Empty;
    }
}
