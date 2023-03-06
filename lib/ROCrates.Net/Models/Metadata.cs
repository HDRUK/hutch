using System.Text.Json;
using System.Text.Json.Nodes;

namespace ROCrates.Models;

/// <summary>
/// RO-Crate Metadata file.
/// </summary>
public class Metadata : File
{
  protected const string BaseName = "ro-crate-metadata.json";
  protected const string Profile = "https://w3id.org/ro/crate/1.1";

  public RootDataset? RootDataset => RoCrate.RootDataset;

  public Metadata(ROCrate crate, string? identifier = null, JsonObject? properties = null, string? source = null,
    string? destPath = null, bool fetchRemote = false, bool validateUrl = false) : base(crate, identifier, properties,
    source, destPath, fetchRemote, validateUrl)
  {
    DefaultType = "CreativeWork";
    Properties = _empty();
    if (properties is not null) _unpackProperties(properties);
    SetProperty("conformsTo", new Dictionary<string, string>{{"@id", Profile}});
    SetProperty("about", new Dictionary<string, string>{{"@id", "./"}});
    Identifier = source ?? destPath ?? BaseName;
  }

  private JsonObject _generate()
  {
    // Iterate through the entities in the RO-Crate, extract their properties and serialise to JSON.
    throw new NotImplementedException();
  }

  public override void Write(string basePath)
  {
    var outPath = Path.Combine(basePath, Identifier);
    var metadataJson = _generate();
    System.IO.File.WriteAllText(outPath, metadataJson.ToString());
  }
}
