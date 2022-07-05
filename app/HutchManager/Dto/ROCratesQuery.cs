using System.Text.Json.Serialization;


namespace HutchManager.Dto;

public class ROCratesQuery
{
  [JsonPropertyName("@context")]
  public string Context { get; set; } = string.Empty;
  
  [JsonPropertyName("@graph")]
  public List<ROCratesGraph> Graphs { get; set; } = new ();

  public class ROCratesGraph
  {
    [JsonPropertyName("@context")]
    public string Context { get; set; } = string.Empty;
    
    [JsonPropertyName("@type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
    
    [JsonPropertyName(("numberOfItems"))]
    public int NumberOfItems { get; set;}
    
    [JsonPropertyName("itemListElement")] 
    public List<Item> ItemListElements { get; set;} = new ();
  }
  
  public class Item
  {
    [JsonPropertyName("@context")]
    public string Context { get; set; } = string.Empty;
    
    [JsonPropertyName("@type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("additionalProperty")]
    public List<Property> AdditionalProperties { get; set; } = new();
  }

  public class Property
  {
    [JsonPropertyName("@context")]
    public string Context { get; set; } = string.Empty;
    
    [JsonPropertyName("@type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
  }
}

