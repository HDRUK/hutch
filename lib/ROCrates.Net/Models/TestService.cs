using System.Text.Json.Nodes;

namespace ROCrates.Models;

public class TestService : ContextEntity
{
  public TestService(ROCrate crate, string? identifier = null, JsonObject? properties = null) : base(crate, identifier,
    properties)
  {
    DefaultType = "TestService";
    Properties = _empty();
    if (properties is not null) _unpackProperties(properties);
  }
  
  public string? Name
  {
    get => GetProperty<string>("name");
    set => SetProperty("name", value);
  }
  
  public string? Url
  {
    get => GetProperty<string>("url");
    set => SetProperty("url", value);
  }
}
