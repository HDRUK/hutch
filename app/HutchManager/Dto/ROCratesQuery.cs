using System.Text.Json.Serialization;


namespace HutchManager.Dto;

public class ROCratesQuery
{
  [JsonPropertyName("@context")]
  public string Context { get; set; } = "https://w3id.org/ro/crate/1.1/context";
  
  [JsonPropertyName("@graph")]
  public List<ROCratesGraph> Graphs { get; set; } = new ();

  public class ROCratesGraph
  {
    [JsonPropertyName("@context")]
    public string Context { get; set; } = "https://schema.org";
    
    [JsonPropertyName("@type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
    
    [JsonPropertyName("numberOfItems")]
    public int? NumberOfItems { get; set;}
    
    [JsonPropertyName("itemListElement")] 
    public IEnumerable<Item> ItemListElements { get; set;} = null!;
  }
  
  public class Item
  {
    [JsonPropertyName( "@context")]
    public string Context { get; set; } = "https://schema.org";
    
    [JsonPropertyName("@type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("additionalProperty")]
    public Property? AdditionalProperty { get; set; }
  }

  public class Property
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
}

