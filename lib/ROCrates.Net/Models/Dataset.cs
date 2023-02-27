using System.Globalization;
using System.Text.Json.Nodes;

namespace ROCrates.Models;

public class Dataset : FileOrDir
{
  public Dataset(ROCrate crate, string? identifier = null, JsonObject? properties = null, string source = "./",
    string? destPath = null, bool fetchRemote = false, bool validateUrl = false) : base(crate, identifier, properties,
    source, destPath, fetchRemote, validateUrl)
  {
    DefaultType = "Dataset";
    Properties = _empty();
  }

  public override void Write(string basePath)
  {
    var outFilePath = Path.Join(basePath, Identifier);
    if (Uri.IsWellFormedUriString(_source, UriKind.Absolute))
    {
      if (_validateUrl && !_fetchRemote)
      {
        using HttpClient client = new HttpClient();
        var response = client.GetAsync(_source).Result.Content;
        SetProperty("sdDatePublished", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
      }

      if (_fetchRemote)
      {
        
      }
    }
  }

  private void _getParts(string outPath)
  {
    
  }
}
