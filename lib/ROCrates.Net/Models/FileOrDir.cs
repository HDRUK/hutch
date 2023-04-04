using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using ROCrates.Converters;

namespace ROCrates.Models;

public class FileOrDir : DataEntity
{
  protected string _source;
  protected string? _destPath;
  protected bool _fetchRemote;
  protected bool _validateUrl;

  public FileOrDir(ROCrate? crate = null, string? identifier = null, JsonObject? properties = null,
    string? source = null,
    string? destPath = null, bool fetchRemote = false, bool validateUrl = false) : base(crate, identifier, properties)
  {
    _source = source ?? "./";
    _destPath = destPath;
    _fetchRemote = fetchRemote;
    _validateUrl = validateUrl;

    if (_destPath != null)
    {
      if (Path.IsPathRooted(_destPath))
      {
        throw new Exception("destPath must be a relative path, not an absolute path.");
      }

      // Convert Windows paths to POSIX
      Id = _destPath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }
    else if (Uri.IsWellFormedUriString(_source, UriKind.RelativeOrAbsolute) && _fetchRemote)
    {
      Id = Path.GetFileNameWithoutExtension(_source);
    }
    else if (Path.GetFileName(_source) != String.Empty)
    {
      Id = Path.GetFileNameWithoutExtension(_source);
    }
    else
    {
      Id = _source;
    }
  }

  public FileOrDir()
  {
    _source = "./";
  }

  /// <summary>
  /// Convert <see cref="FileOrDir"/> to JSON string.
  /// </summary>
  /// <returns>The <see cref="FileOrDir"/> as a JSON string.</returns>
  public override string Serialize()
  {
    var options = new JsonSerializerOptions
    {
      WriteIndented = true,
      Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
      Converters = { new EntityConverter<FileOrDir>() }
    };
    var serialised = JsonSerializer.Serialize(this, options);
    return serialised;
  }

  /// <summary>
  /// Create a <see cref="FileOrDir"/> from JSON properties.
  /// </summary>
  /// <param name="entityJson">The JSON representing the <see cref="FileOrDir"/></param>
  /// <param name="roCrate">The RO-Crate for the <see cref="FileOrDir"/></param>
  /// <returns>The deserialised <see cref="FileOrDir"/></returns>
  public new static FileOrDir? Deserialize(string entityJson, ROCrate roCrate)
  {
    var options = new JsonSerializerOptions
    {
      WriteIndented = true,
      Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
      Converters = { new EntityConverter<FileOrDir>() }
    };
    var deserialized = JsonSerializer.Deserialize<FileOrDir>(entityJson, options);
    if (deserialized is not null) deserialized.RoCrate = roCrate;
    return deserialized;
  }
}
