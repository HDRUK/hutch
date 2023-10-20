using System.IO;
using System.IO.Abstractions;
using System.Text.RegularExpressions;

namespace HutchAgent.Utilities;

public class FileSystemUtility
{
  private readonly IFileSystem _fs;

  public FileSystemUtility(IFileSystem fs)
  {
    _fs = fs;
  }

  /// <summary>
  /// Delete everything from a directory (non-recursive)
  /// EXCEPT files which match a provided pattern.
  /// </summary>
  /// <param name="targetPath">Path to the directory to delete files from</param>
  /// <param name="keepPattern">A regex pattern for file paths to keep</param>
  public void SelectivelyDelete(string targetPath, Regex keepPattern)
  {
    foreach (var filePath in _fs.Directory.EnumerateFiles(targetPath))
    {
      if (!keepPattern.IsMatch(filePath))
        _fs.File.Delete(filePath);
    }
  }

  // https://learn.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories
  /// <summary>
  /// Copy a directory, optionally including all subdirectories recursively.
  /// </summary>
  /// <param name="sourceDir">The path to the directory to copy</param>
  /// <param name="destinationDir">The path to copy to</param>
  /// <param name="recursive">Whether to recursivly copy subdirectories</param>
  /// <exception cref="DirectoryNotFoundException"></exception>
  public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
  {
    // Get information about the source directory
    var dir = new DirectoryInfo(sourceDir);

    // Check if the source directory exists
    if (!dir.Exists)
      throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

    // Cache directories before we start copying
    var dirs = dir.GetDirectories();

    // Create the destination directory
    Directory.CreateDirectory(destinationDir);

    // Get the files in the source directory and copy to the destination directory
    foreach (var file in dir.GetFiles())
    {
      var targetFilePath = Path.Combine(destinationDir, file.Name);
      file.CopyTo(targetFilePath);
    }

    // If recursive and copying subdirectories, recursively call this method
    if (recursive)
    {
      foreach (var subDir in dirs)
      {
        var newDestinationDir = Path.Combine(destinationDir, subDir.Name);
        CopyDirectory(subDir.FullName, newDestinationDir, true);
      }
    }
  }
}
