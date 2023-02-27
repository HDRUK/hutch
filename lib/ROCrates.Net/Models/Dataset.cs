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
    var outFileParent = Path.GetDirectoryName(outFilePath);
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
        _getParts(basePath);
      }
    }
    else
    {
      // Copy files on system
    }
  }

  private void _getParts(string outPath)
  {
    Directory.CreateDirectory(outPath);
    var basePath = _source.TrimEnd('/');
    var parts = GetProperty<Dictionary<string, string>>("hasPart");
    if (parts is null) return;
    using var httpClient = new HttpClient();
    foreach (var p in parts)
    {
      var part = p.Key == "@id" ? p.Value : null;
      if (part is null) continue;
      var partUri = $"{basePath}/{part}";
      var partOutPath = Path.Combine(outPath, part);
      var response = httpClient.GetAsync(partUri).Result.Content;
      using var httpStream = response.ReadAsStream();
      using var file = System.IO.File.OpenWrite(partOutPath);
      httpStream.CopyTo(file);
    }
  }
}
