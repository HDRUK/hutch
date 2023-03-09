using System.Text.Json.Nodes;

namespace ROCrates.Models;

public class TestInstance : ContextEntity
{
  public TestInstance(ROCrate crate, string? identifier = null, JsonObject? properties = null) : base(crate, identifier,
    properties)
  {
    DefaultType = "TestInstance";
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
  
  public string? Resource
  {
    get => GetProperty<string>("resource");
    set => SetProperty("resource", value);
  }
  
  public TestService? RunsOn
  {
    get => GetProperty<TestService>("runsOn");
    set => SetProperty("runsOn", value);
  }
}
