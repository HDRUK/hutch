using System.Text.Json;
using System.Text.Json.Nodes;
using ROCrates.Converters;

namespace ROCrates.Models;

/// <summary>
/// RO-Crate Metadata file.
/// </summary>
public class Metadata : File
{
  protected const string BaseName = "ro-crate-metadata.json";
  protected const string Profile = "https://w3id.org/ro/crate/1.1";

  public RootDataset? RootDataset => RoCrate.RootDataset;
  public JsonObject? ExtraTerms { get; set; }

  public Metadata(ROCrate? crate = null, string? identifier = null, JsonObject? properties = null,
    string? source = null,
    string? destPath = null, bool fetchRemote = false, bool validateUrl = false) : base(crate, identifier, properties,
    source, destPath, fetchRemote, validateUrl)
  {
    DefaultType = "CreativeWork";
    Properties = _empty();
    if (properties is not null) _unpackProperties(properties);
    SetProperty("conformsTo", new Dictionary<string, string> { { "@id", Profile } });
    SetProperty("about", new Dictionary<string, string> { { "@id", "./" } });
    Id = source ?? destPath ?? BaseName;
  }

  private JsonObject _generate()
  {
    // Iterate through the entities in the RO-Crate, extract their properties and serialise to JSON.
    throw new NotImplementedException();
  }

  public override void Write(string basePath)
  {
    var outPath = Path.Combine(basePath, Id);
    var metadataJson = _generate();
    System.IO.File.WriteAllText(outPath, metadataJson.ToString());
  }

  /// <summary>
  /// Convert <see cref="Metadata"/> to JSON string.
  /// </summary>
  /// <returns>The <see cref="Metadata"/> as a JSON string.</returns>
  public override string Serialize()
  {
    var options = new JsonSerializerOptions
    {
      WriteIndented = true,
      Converters = { new MetadataConverter() }
    };
    var serialised = JsonSerializer.Serialize(this, options);
    return serialised;
  }

  /// <summary>
  /// Create a <see cref="Metadata"/> from JSON properties.
  /// </summary>
  /// <param name="entityJson">The JSON representing the <see cref="Metadata"/></param>
  /// <param name="roCrate">The RO-Crate for the <see cref="Metadata"/></param>
  /// <returns>The deserialised <see cref="Metadata"/></returns>
  public new static Metadata? Deserialize(string entityJson, ROCrate roCrate)
  {
    var options = new JsonSerializerOptions
    {
      WriteIndented = true,
      Converters = { new MetadataConverter() }
    };
    var deserialized = JsonSerializer.Deserialize<Metadata>(entityJson, options);
    if (deserialized is not null) deserialized.RoCrate = roCrate;
    return deserialized;
  }
}
