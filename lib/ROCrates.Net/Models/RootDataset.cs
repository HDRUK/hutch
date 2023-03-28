using System.Text.Json;
using System.Text.Json.Nodes;
using ROCrates.Converters;

namespace ROCrates.Models;

public class RootDataset : Dataset
{
  public RootDataset(ROCrate? crate = null, string? identifier = null, JsonObject? properties = null,
    string? source = null,
    string? destPath = null, bool fetchRemote = false, bool validateUrl = false) : base(crate, identifier, properties,
    source, destPath, fetchRemote, validateUrl)
  {
    Properties = _empty();
    if (properties is not null) _unpackProperties(properties);
  }

  /// <summary>
  /// Convert <see cref="RootDataset"/> to JSON string.
  /// </summary>
  /// <returns>The <see cref="RootDataset"/> as a JSON string.</returns>
  public override string Serialize()
  {
    var options = new JsonSerializerOptions
    {
      WriteIndented = true,
      Converters = { new RootDatasetConverter() }
    };
    var serialised = JsonSerializer.Serialize(this, options);
    return serialised;
  }

  /// <summary>
  /// Create a <see cref="RootDataset"/> from JSON properties.
  /// </summary>
  /// <param name="entityJson">The JSON representing the <see cref="RootDataset"/></param>
  /// <param name="roCrate">The RO-Crate for the <see cref="RootDataset"/></param>
  /// <returns>The deserialised <see cref="RootDataset"/></returns>
  public new static RootDataset? Deserialize(string entityJson, ROCrate roCrate)
  {
    var options = new JsonSerializerOptions
    {
      WriteIndented = true,
      Converters = { new RootDatasetConverter() }
    };
    var deserialized = JsonSerializer.Deserialize<RootDataset>(entityJson, options);
    if (deserialized is not null) deserialized.RoCrate = roCrate;
    return deserialized;
  }
}
