using System.Text.Json;
using System.Text.Json.Nodes;

namespace ROCrates.Models;

public class File : FileOrDir
{
  private const string _defaultType = "File";
  
  public File(ROCrate crate, string? identifier = null, JsonObject? properties = null, string source = "./",
    string? destPath = null, bool fetchRemote = false, bool validateUrl = false) : base(crate, identifier, properties,
    source, destPath, fetchRemote, validateUrl)
  {
    Properties = _empty();
    if (properties != null)
    {
      using var propsEnumerator = properties.GetEnumerator();
      while (propsEnumerator.MoveNext())
      {
        var (key, value) = propsEnumerator.Current;
        if (value != null) SetProperty(key, value);
      }
    }
  }

  public void Write(string basePath)
  {
    string outFilePath = Path.Join(basePath, Identifier);
    if (Uri.IsWellFormedUriString(_source, UriKind.RelativeOrAbsolute))
    {
      var url = new Uri(_source);
      using HttpClient client = new HttpClient();
      var response = client.GetAsync(_source);
    }
    var outFileParent = Path.GetDirectoryName(outFilePath);
    if (outFileParent != null)
    {
      Directory.CreateDirectory(outFileParent);
      System.IO.File.Copy(_source, outFilePath);
    }
  }
  private JsonObject _empty()
  {
    var emptyJsonString = new Dictionary<string, string>
    {
      { "@id", Identifier },
      { "@type", _defaultType }
    };
    var emptyObject = JsonSerializer.SerializeToNode(emptyJsonString).AsObject();
    return emptyObject;
  }
}
