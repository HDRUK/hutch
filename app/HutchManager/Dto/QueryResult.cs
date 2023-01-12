using System.Text.Json.Serialization;

namespace HutchManager.Dto;

/// <summary>
/// This class represents the overall result of an RQuest job.
/// </summary>
public class RquestQueryResult
{
  [JsonPropertyName("status")]
  public string Status { get; set; } = string.Empty;
  public string ProtocolVersion { get; set; } = "v2";

  [JsonPropertyName("collection_id")]
  public int CollectionId { get; set; }

  [JsonPropertyName("uuid")] 
  public string Uuid { get; set; } = string.Empty;

  [JsonPropertyName("queryResult")]
  public QueryResult Results { get; set; } = new();

  [JsonPropertyName("message")]
  public string? Message { get; set; } = null;
}

/// <summary>
/// This class represents the <c>queryResult</c> field in an <c>RquestQueryResult</c> object.
/// <seealso cref="RquestQueryResult"/>
/// </summary>
public class QueryResult
{
  [JsonPropertyName("count")]
  public int Count { get; set; } = 0;
  
  [JsonPropertyName("datasetCount")]
  public int DatasetCount { get; set; } = 0;
  
  [JsonPropertyName("files")]
  public List<RquestFile> Files { get; set; } = new();
}

/// <summary>
/// This class represents an individual file in the list <c>QueryResult.Files</c>.
/// <seealso cref="QueryResult"/>
/// </summary>
public class RquestFile
{
  [JsonPropertyName("file_data")]
  public string FileData { get; set; } = string.Empty;
  
  [JsonPropertyName("file_name")]
  public string FileName { get; set; } = string.Empty;
  
  [JsonPropertyName("file_description")]
  public string? FileDescription { get; set; } = null;
  
  [JsonPropertyName("file_reference")]
  public string FileReference { get; set; } = string.Empty;
  
  [JsonPropertyName("file_sensitive")]
  public bool FileSensitive { get; set; } = true;
  
  [JsonPropertyName("file_size")]
  public double FileSize { get; set; } = 0.0;
  
  [JsonPropertyName("file_type")]
  public string FileType { get; set; } = "BCOS";
}
