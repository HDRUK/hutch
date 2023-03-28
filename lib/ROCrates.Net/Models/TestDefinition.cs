using System.Text.Json;
using System.Text.Json.Nodes;
using ROCrates.Converters;

namespace ROCrates.Models;

public class TestDefinition : File
{
  public TestDefinition(ROCrate? crate = null, string? identifier = null, JsonObject? properties = null,
    string? source = null,
    string? destPath = null, bool fetchRemote = false, bool validateUrl = false) : base(crate, identifier, properties,
    source, destPath, fetchRemote, validateUrl)
  {
    DefaultType = "TestDefinition";
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

  private new JsonObject _empty()
  {
    var emptyJsonString = new Dictionary<string, string>
    {
      { "@id", Id },
    };
    var emptyObject = JsonSerializer.SerializeToNode(emptyJsonString).AsObject();
    var typesList = new List<string> { "File", "TestDefinition" };
    var serialisedList = JsonSerializer.Serialize(typesList);
    emptyObject.Add("@type", serialisedList);
    return emptyObject;
  }

  /// <summary>
  /// Convert <see cref="TestDefinition"/> to JSON string.
  /// </summary>
  /// <returns>The <see cref="TestDefinition"/> as a JSON string.</returns>
  public override string Serialize()
  {
    var options = new JsonSerializerOptions
    {
      WriteIndented = true,
      Converters = { new TestDefinitionConverter() }
    };
    var serialised = JsonSerializer.Serialize(this, options);
    return serialised;
  }

  public new static TestDefinition? Deserialize(string entityJson, ROCrate roCrate)
  {
    var options = new JsonSerializerOptions
    {
      WriteIndented = true,
      Converters = { new TestDefinitionConverter() }
    };
    var deserialized = JsonSerializer.Deserialize<TestDefinition>(entityJson, options);
    if (deserialized is not null) deserialized.RoCrate = roCrate;
    return deserialized;
  }
}
