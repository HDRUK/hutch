using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using ROCrates.Converters;

namespace ROCrates.Models;

/// <summary>
/// This class represents a generic file entity that can go inside an RO-Crate.
/// </summary>
public class File : FileOrDir
{
  public File(ROCrate? crate = null, string? identifier = null, JsonObject? properties = null, string? source = null,
    string? destPath = null, bool fetchRemote = false, bool validateUrl = false) : base(crate, identifier, properties,
    source, destPath, fetchRemote, validateUrl)
  {
    DefaultType = "File";
    Properties = _empty();
    if (properties is not null) _unpackProperties(properties);
  }

  /// <summary>
  ///   <para>Write file contents to the specified path. e.g. The root path of an RO-Crate.</para>
  ///   <para>
  ///     If the file is a remote file, and <c>fetchUrl</c> is set to <c>true</c>, the file will be downloaded under
  ///     "<c>basePath</c>".
  ///   </para>
  ///   <para>
  ///     If the file is on disk, it will be copied to a new location under "<c>basePath</c>".
  ///   </para>
  ///   <para>
  ///     In either case, the file will be saved to "<c>basePath/Id</c>"
  ///   </para>
  /// </summary>
  /// <example>
  /// <code>
  /// var url = "https://hdruk.github.io/hutch/docs/devs";
  /// var fileName = url.Split('/').Last();
  /// var fileEntity = new Models.File(
  ///    new ROCrate("myCrate.zip"),
  ///    source: url,
  ///    validateUrl: true,
  ///    fetchRemote: true);
  /// fileEntity.Write("myCrate");
  /// Assert.True(File.Exists(Path.Combine("myCrate", fileName)));
  /// </code>
  /// </example>
  /// <param name="basePath">The path the file will be written to.</param>
  public override void Write(string basePath)
  {
    var outFilePath = Path.Join(basePath, Id);
    var outFileParent = Path.GetDirectoryName(outFilePath);
    if (outFileParent == null) return;
    if (Uri.IsWellFormedUriString(_source, UriKind.Absolute))
    {
      if (!_fetchRemote && !_validateUrl) return;
      using HttpClient client = new HttpClient();
      var response = client.GetAsync(_source).Result.Content;
      if (_validateUrl)
      {
        SetProperty("contentSize", response.Headers.ContentLength);
        SetProperty("encodingFormat", response.Headers.ContentType);
        if (!_fetchRemote)
          SetProperty("sdDatePublished", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
      }

      if (!_fetchRemote) return;
      Directory.CreateDirectory(outFileParent);
      using var httpStream = response.ReadAsStream();
      using var file = System.IO.File.OpenWrite(outFilePath);
      httpStream.CopyTo(file);
    }
    else
    {
      Directory.CreateDirectory(outFileParent);
      System.IO.File.Copy(_source, outFilePath, overwrite: true);
    }
  }

  /// <summary>
  /// Convert <see cref="File"/> to JSON string.
  /// </summary>
  /// <returns>The <see cref="File"/> as a JSON string.</returns>
  public override string Serialize()
  {
    var options = new JsonSerializerOptions
    {
      WriteIndented = true,
      Converters = { new FileConverter() }
    };
    var serialised = JsonSerializer.Serialize(this, options);
    return serialised;
  }

  public new static File? Deserialize(string entityJson, ROCrate roCrate)
  {
    var options = new JsonSerializerOptions
    {
      WriteIndented = true,
      Converters = { new ContextEntityConverter() }
    };
    var deserialized = JsonSerializer.Deserialize<File>(entityJson, options);
    if (deserialized is not null) deserialized.RoCrate = roCrate;
    return deserialized;
  }
}
