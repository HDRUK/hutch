using System.Text.Json;
using System.Text.Json.Nodes;

namespace ROCrates.Models;

public class Person : ContextEntity
{
  private const string _defaultType = "Person";
  
  public Person(ROCrate crate, string? identifier = null, JsonObject? properties = null) : base(crate, identifier,
    properties)
  {
    Properties = _empty();
  }
  
  private JsonObject _empty()
  {
    var emptyJsonString = new Dictionary<string, string>
    {
      { "@id", Identifier },
      { "@type", _defaultType }
    };
    var emptyObject = JsonSerializer.SerializeToNode(emptyJsonString).AsObject();
    return emptyObject;
  }
}
