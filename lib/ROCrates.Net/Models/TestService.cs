using System.Text.Json;
using System.Text.Json.Nodes;
using ROCrates.Converters;

namespace ROCrates.Models;

public class TestService : ContextEntity
{
  public TestService(ROCrate? crate = null, string? identifier = null, JsonObject? properties = null) : base(crate,
    identifier,
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

  /// <summary>
  /// Convert <see cref="TestService"/> to JSON string.
  /// </summary>
  /// <returns>The <see cref="TestService"/> as a JSON string.</returns>
  public override string Serialize()
  {
    var options = new JsonSerializerOptions
    {
      WriteIndented = true,
      Converters = { new TestServiceConverter() }
    };
    var serialised = JsonSerializer.Serialize(this, options);
    return serialised;
  }

  public new static TestService? Deserialize(string entityJson, ROCrate roCrate)
  {
    var options = new JsonSerializerOptions
    {
      WriteIndented = true,
      Converters = { new TestServiceConverter() }
    };
    var deserialized = JsonSerializer.Deserialize<TestService>(entityJson, options);
    if (deserialized is not null) deserialized.RoCrate = roCrate;
    return deserialized;
  }
}
