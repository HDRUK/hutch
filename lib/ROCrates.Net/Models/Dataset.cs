using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using ROCrates.Converters;

namespace ROCrates.Models;

public class Dataset : FileOrDir
{
  public Dataset(ROCrate? crate = null, string? identifier = null, JsonObject? properties = null, string? source = null,
    string? destPath = null, bool fetchRemote = false, bool validateUrl = false) : base(crate, identifier, properties,
    source, destPath, fetchRemote, validateUrl)
  {
    DefaultType = "Dataset";
    Properties = _empty();
    if (properties is not null) _unpackProperties(properties);
    Id = _formatIdentifier(Id);
  }

  public Dataset()
  {
    DefaultType = "Dataset";
    Properties = _empty();
    Id = _formatIdentifier(Id);
  }

  /// <summary>
  /// <para>Write the contents of a <c>Dataset</c> to disk.</para>
  /// <para>If the <c>Dataset</c>'s source is remote, the contents will be downloaded to <c>basePath</c>.</para>
  /// <para>If the source is on disk, the contents will be copied under <c>basePath</c>.</para>
  /// </summary>
  /// <example>
  /// <code>
  /// var url = "https://hdruk.github.io/hutch/docs/devs";
  /// var dirName = url.Split('/').Last();
  /// var dataset = new Models.Dataset(
  ///    new ROCrate("myCrate.zip"),
  ///    source: url,
  ///    validateUrl: true,
  ///    fetchRemote: true);
  /// dataset.Write("myCrate");
  /// Assert.True(Directory.Exists(Path.Combine("myCrate", dirName)));
  /// </code>
  /// </example>
  /// <param name="basePath">The path under which the <c>Dataset</c>'s parts will be written.</param>
  /// <exception cref="DirectoryNotFoundException">
  /// Thrown when the source directory is not a URL, but it doesn't exist.
  /// </exception>
  public override void Write(string basePath)
  {
    var outPath = Path.Join(basePath, Id);
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
        _getParts(outPath);
      }
    }
    else
    {
      if (!Directory.Exists(_source)) throw new DirectoryNotFoundException($"{_source} does not exist.");
      Directory.CreateDirectory(outPath);
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

  protected new JsonObject _empty()
  {
    var emptyJsonString = new Dictionary<string, string>
    {
      { "@id", Id },
      { "@type", DefaultType },
      { "datePublished", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture) }
    };
    var emptyObject = JsonSerializer.SerializeToNode(emptyJsonString).AsObject();
    return emptyObject;
  }

  private protected sealed override string _formatIdentifier(string identifier)
  {
    var newId = identifier.TrimEnd(Path.DirectorySeparatorChar);
    newId = newId.TrimEnd(Path.AltDirectorySeparatorChar);
    newId += "/";
    return newId;
  }

  /// <summary>
  /// Convert <see cref="Dataset"/> to JSON string.
  /// </summary>
  /// <returns>The <see cref="Dataset"/> as a JSON string.</returns>
  public override string Serialize()
  {
    var options = new JsonSerializerOptions
    {
      WriteIndented = true,
      Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
      Converters = { new EntityConverter<Dataset>() }
    };
    var serialised = JsonSerializer.Serialize(this, options);
    return serialised;
  }

  /// <summary>
  /// Create a <see cref="Dataset"/> from JSON properties.
  /// </summary>
  /// <param name="entityJson">The JSON representing the <see cref="Dataset"/></param>
  /// <param name="roCrate">The RO-Crate for the <see cref="Dataset"/></param>
  /// <returns>The deserialised <see cref="Dataset"/></returns>
  public new static Dataset? Deserialize(string entityJson, ROCrate roCrate)
  {
    var options = new JsonSerializerOptions
    {
      WriteIndented = true,
      Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
      Converters = { new EntityConverter<Dataset>() }
    };
    var deserialized = JsonSerializer.Deserialize<Dataset>(entityJson, options);
    if (deserialized is not null) deserialized.RoCrate = roCrate;
    return deserialized;
  }
}
