using System.Text.Json.Nodes;

namespace ROCrates.Models;

public class FileOrDir : DataEntity
{
  protected string _source;
  protected string? _destPath;
  protected bool _fetchRemote;
  protected bool _validateUrl;
  public FileOrDir(ROCrate crate, string? identifier = null, JsonObject? properties = null, string source = "./",
    string? destPath = null, bool fetchRemote = false, bool validateUrl = false) : base(crate, identifier, properties)
  {
    _source = source;
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
      Identifier = _destPath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }
    else if (Uri.IsWellFormedUriString(_source, UriKind.RelativeOrAbsolute) && _fetchRemote)
    {
      Identifier = Path.GetFileNameWithoutExtension(_source);
    }
    else if (Path.GetFileName(_source) != String.Empty)
    {
      Identifier = Path.GetFileNameWithoutExtension(_source);
    }
    else
    {
      Identifier = _source;
    }
  }
}
