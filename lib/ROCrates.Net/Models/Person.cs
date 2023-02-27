using System.Text.Json;
using System.Text.Json.Nodes;

namespace ROCrates.Models;

public class Person : ContextEntity
{
  public Person(ROCrate crate, string? identifier = null, JsonObject? properties = null) : base(crate, identifier,
    properties)
  {
    DefaultType = "Person";
    Properties = _empty();
    if (properties is not null) _unpackProperties(properties);
  }
}
