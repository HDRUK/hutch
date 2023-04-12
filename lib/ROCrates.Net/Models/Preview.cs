using System.Text.Json;
using System.Text.Json.Nodes;

namespace ROCrates.Models;

public class Preview : File
{
  protected const string FileName = "ro-crate-metadata.json";

  public Preview()
  {
    DefaultType = "CreativeWork";
    Properties = _empty();
  }

  public Preview(ROCrate? crate = null, string? identifier = null, JsonObject? properties = null, string? source = null,
    string? destPath = null, bool fetchRemote = false, bool validateUrl = false) : base(crate, identifier, properties,
    source, destPath, fetchRemote, validateUrl)
  {
    DefaultType = "CreativeWork";
    Properties = _empty();
    if (properties is not null) _unpackProperties(properties);
  }

  protected new JsonObject _empty()
  {
    var emptyJsonString = new Dictionary<string, string>
    {
      { "@id", FileName },
      { "@type", DefaultType },
      { "about", "./" }
    };
    var emptyObject = JsonSerializer.SerializeToNode(emptyJsonString).AsObject();
    return emptyObject;
  }
}
