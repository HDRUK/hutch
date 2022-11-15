using System.Text.Json;
using System.Text.Json.Serialization;

namespace HutchManager.Dto;

public class ROCratesQueryResult
{
  [JsonPropertyName("@context")]
  public string Context { get; set; } = "https://w3id.org/ro/crate/1.1/context";
  
  [JsonPropertyName("@graph")]
  public List<JsonElement> Graphs { get; set; } = new ();

}
