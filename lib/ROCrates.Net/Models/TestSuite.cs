using System.Text.Json;
using System.Text.Json.Nodes;
using ROCrates.Converters;

namespace ROCrates.Models;

public class TestSuite : ContextEntity
{
  public TestSuite(ROCrate crate, string? identifier = null, JsonObject? properties = null) : base(crate, identifier,
    properties)
  {
    DefaultType = "TestSuite";
    Properties = _empty();
    if (properties is not null) _unpackProperties(properties);
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
      Converters = { new TestSuiteConverter() }
    };
    var serialised = JsonSerializer.Serialize(this, options);
    return serialised;
  }
}
