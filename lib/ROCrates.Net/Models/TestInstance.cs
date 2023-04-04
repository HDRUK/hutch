using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using ROCrates.Converters;

namespace ROCrates.Models;

public class TestInstance : ContextEntity
{
  public TestInstance(ROCrate? crate = null, string? identifier = null, JsonObject? properties = null) : base(crate,
    identifier,
    properties)
  {
    DefaultType = "TestInstance";
    Properties = _empty();
    if (properties is not null) _unpackProperties(properties);
  }

  public TestInstance()
  {
    DefaultType = "TestInstance";
    Properties = _empty();
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

  /// <summary>
  /// Convert <see cref="TestInstance"/> to JSON string.
  /// </summary>
  /// <returns>The <see cref="TestInstance"/> as a JSON string.</returns>
  public override string Serialize()
  {
    var options = new JsonSerializerOptions
    {
      WriteIndented = true,
      Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
      Converters = { new EntityConverter<TestInstance>() }
    };
    var serialised = JsonSerializer.Serialize(this, options);
    return serialised;
  }

  /// <summary>
  /// Create a <see cref="TestInstance"/> from JSON properties.
  /// </summary>
  /// <param name="entityJson">The JSON representing the <see cref="TestInstance"/></param>
  /// <param name="roCrate">The RO-Crate for the <see cref="TestInstance"/></param>
  /// <returns>The deserialised <see cref="TestInstance"/></returns>
  public new static TestInstance? Deserialize(string entityJson, ROCrate roCrate)
  {
    var options = new JsonSerializerOptions
    {
      WriteIndented = true,
      Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
      Converters = { new EntityConverter<TestInstance>() }
    };
    var deserialized = JsonSerializer.Deserialize<TestInstance>(entityJson, options);
    if (deserialized is not null) deserialized.RoCrate = roCrate;
    return deserialized;
  }
}
