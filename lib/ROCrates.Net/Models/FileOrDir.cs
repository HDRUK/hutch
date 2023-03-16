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

  public FileOrDir(ROCrate crate, string? identifier = null, JsonObject? properties = null, string? source = null,
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

  /// <summary>
  /// Convert <see cref="FileOrDir"/> to JSON string.
  /// </summary>
  /// <returns>The <see cref="FileOrDir"/> as a JSON string.</returns>
  public override string Serialize()
  {
    var options = new JsonSerializerOptions
    {
      WriteIndented = true,
      Converters = { new FileOrDirConverter() }
    };
    var serialised = JsonSerializer.Serialize(this, options);
    return serialised;
  }
}
