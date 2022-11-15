using System.Text.Json.Serialization;

namespace HutchManager.Dto;

public class ROCratesQueryResult
{
  [JsonPropertyName("@context")]
  public string Context { get; set; } = "https://w3id.org/ro/crate/1.1/context";
  
  [JsonPropertyName("@graph")]
  public List<PropertyValue> Graphs { get; set; } = new ();

}

public class PropertyValue
{
  [JsonPropertyName("@context")]
  public string Context { get; set; } = "https://schema.org";
    
  [JsonPropertyName("@type")]
  public string Type { get; set; } = string.Empty;
    
  [JsonPropertyName("name")]
  public string Name { get; set; } = string.Empty;
    
  [JsonPropertyName("value")]
  public string Value { get; set; } = string.Empty;

}
