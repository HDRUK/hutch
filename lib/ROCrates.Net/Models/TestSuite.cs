using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using ROCrates.Converters;

namespace ROCrates.Models;

public class TestSuite : ContextEntity
{
  public TestSuite(ROCrate? crate = null, string? identifier = null, JsonObject? properties = null) : base(crate,
    identifier,
    properties)
  {
    DefaultType = "TestSuite";
    Properties = _empty();
    if (properties is not null) _unpackProperties(properties);
  }

  public TestSuite()
  {
    DefaultType = "TestSuite";
    Properties = _empty();
  }

  public string? EngineVersion
  {
    get => GetProperty<string>("engineVersion");
    set => SetProperty("engineVersion", value);
  }

  public Part? ConformsTo
  {
    get => GetProperty<Part>("conformsTo");
    set => SetProperty("conformsTo", value);
  }

  public string? Name
  {
    get => GetProperty<string>("name");
    set => SetProperty("name", value);
  }

  /// <summary>
  /// Convert <see cref="TestSuite"/> to JSON string.
  /// </summary>
  /// <returns>The <see cref="TestSuite"/> as a JSON string.</returns>
  public override string Serialize()
  {
    var options = new JsonSerializerOptions
    {
      WriteIndented = true,
      Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
      Converters = { new EntityConverter<TestSuite>() }
    };
    var serialised = JsonSerializer.Serialize(this, options);
    return serialised;
  }

  /// <summary>
  /// Create a <see cref="TestSuite"/> from JSON properties.
  /// </summary>
  /// <param name="entityJson">The JSON representing the <see cref="TestSuite"/></param>
  /// <param name="roCrate">The RO-Crate for the <see cref="TestSuite"/></param>
  /// <returns>The deserialised <see cref="TestSuite"/></returns>
  public new static TestSuite? Deserialize(string entityJson, ROCrate roCrate)
  {
    var options = new JsonSerializerOptions
    {
      WriteIndented = true,
      Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
      Converters = { new EntityConverter<TestSuite>() }
    };
    var deserialized = JsonSerializer.Deserialize<TestSuite>(entityJson, options);
    if (deserialized is not null) deserialized.RoCrate = roCrate;
    return deserialized;
  }
}
