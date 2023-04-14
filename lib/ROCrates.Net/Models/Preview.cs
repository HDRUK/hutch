using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Scriban;

namespace ROCrates.Models;

public class Preview : File
{
  protected const string FileName = "ro-crate-preview.html";

  public Preview()
  {
    DefaultType = "CreativeWork";
    Id = FileName;
    Properties = _empty();
  }

  public Preview(ROCrate? crate = null, string? identifier = null, JsonObject? properties = null, string? source = null,
    string? destPath = null, bool fetchRemote = false, bool validateUrl = false) : base(crate, identifier, properties,
    source, destPath, fetchRemote, validateUrl)
  {
    DefaultType = "CreativeWork";
    Id = source ?? destPath ?? FileName;
    Properties = _empty();
    if (properties is not null) _unpackProperties(properties);
  }

  /// <summary>
  /// Write the HTML preview of the RO-Crate.
  /// </summary>
  /// <param name="basePath">The directory where the preview file will be written.</param>
  public override void Write(string basePath)
  {
    var templatePath = Path.Combine(
      Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
      "Templates/ro-crates-preview.html");
    var templateString = System.IO.File.ReadAllText(templatePath);
    var template = Template.Parse(templateString);

    var data = from entity in RoCrate.Entities.Values select entity.Properties;
    var result = template.Render(new { data = data, root_dataset = RoCrate?.RootDataset });

    System.IO.File.WriteAllText(Path.Combine(basePath, FileName), result);
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
