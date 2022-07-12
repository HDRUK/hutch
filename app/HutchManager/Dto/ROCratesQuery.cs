using System.Text.Json.Serialization;
using Newtonsoft.Json;


namespace HutchManager.Dto;

public class ROCratesQuery
{
  [JsonProperty(PropertyName="@context")]
  public string Context { get; set; } = "https://w3id.org/ro/crate/1.1/context";
  
  [JsonProperty(PropertyName="@graph")]
  public List<ROCratesGraph> Graphs { get; set; } = new ();

  public class ROCratesGraph
  {
    [JsonProperty(PropertyName="@context")]
    public string Context { get; set; } = "https://schema.org";
    
    [JsonProperty(PropertyName="@type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonProperty(PropertyName="name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonProperty(PropertyName="value")]
    public string Value { get; set; } = string.Empty;
    
    [JsonProperty(PropertyName="numberOfItems")]
    public int? NumberOfItems { get; set;}
    
    [JsonProperty(PropertyName="itemListElement")] 
    public IEnumerable<Item> ItemListElements { get; set;} = null!;
  }
  
  public class Item
  {
    [JsonProperty(PropertyName = "@context")]
    public string Context { get; set; } = "https://schema.org";
    
    [JsonProperty(PropertyName ="@type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonProperty(PropertyName="name")]
    public string? Name { get; set; }
    
    [JsonProperty(PropertyName="value")]
    public string Value { get; set; } = string.Empty;

    [JsonProperty(PropertyName="additionalProperty")]
    public List<Property>? AdditionalProperties { get; set; }
  }

  public class Property
  {
    [JsonProperty(PropertyName="@context")]
    public string Context { get; set; } = "https://schema.org";
    
    [JsonProperty(PropertyName="@type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonProperty(PropertyName="name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonProperty(PropertyName="value")]
    public string Value { get; set; } = string.Empty;
  }
}

