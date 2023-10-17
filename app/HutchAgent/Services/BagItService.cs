using HutchAgent.Utilities;

namespace HutchAgent.Services;

public static class BagItUtilities
{
  public const string RelativePayloadPath = "data";

  /// <summary>
  /// Given a path to the root of a BagIt directory, return the path to its payload.
  /// </summary>
  /// <param name="bagItPath">Path to the root of a BagIt directory.</param>
  /// <returns></returns>
  public static string BagItPayloadPath(this string bagItPath) => Path.Combine(bagItPath, RelativePayloadPath);
}

/// <summary>
/// Service for Hutch operations on BagIt archives.
/// </summary>
public class BagItService
{
  private const string _manifestName = "manifest-sha512.txt";
  private const string _tagManifestName = "tagmanifest-sha512.txt";

  private static string[] _tagFiles =
    { "bagit.txt", "bagit-info.txt", "manifest-sha512.txt" };

  /// <summary>
  /// Compute the SHA512 for each file in the Bagit archive's <c>data</c> subdirectory and write a
  /// <c>manifest-sha512.txt</c> to the archive.
  /// </summary>
  /// <param name="bagitDir">The path to the Bagit archive.</param>
  public async Task WriteManifestSha512(string bagitDir)
  {
    await using var manifestFile =
      new FileStream(Path.Combine(bagitDir, _manifestName), FileMode.Create, FileAccess.Write);
    await using var writer = new StreamWriter(manifestFile);
    foreach (var entry in Directory.EnumerateFiles(bagitDir.BagItPayloadPath(), "*", SearchOption.AllDirectories))
    {
      await using var stream = new FileStream(entry, FileMode.Open, FileAccess.Read);
      var checksum = ChecksumUtility.ComputeSha512(stream);
      // Note there should be 2 spaces between the checksum and the file path
      // The path should be relative to bagitDir
      var path = Path.GetRelativePath(bagitDir, entry);
      await writer.WriteLineAsync($"{checksum}  {path}");
    }
  }

  /// <summary>
  /// Compute the SHA512 for the <c>bagit.txt</c>, <c>bag-info.txt</c> and <c>manifest-sha512.txt</c> and
  /// write a <c>tagmanifest-sha512.txt</c> to the archive.
  /// </summary>
  /// <param name="bagitDir">The path to the Bagit archive.</param>
  /// <exception cref="FileNotFoundException">Thrown if a tag file doesn't exist in the archive.</exception>
  public async Task WriteTagManifestSha512(string bagitDir)
  {
    await using var manifestFile =
      new FileStream(Path.Combine(bagitDir, _tagManifestName), FileMode.Create, FileAccess.Write);
    await using var writer = new StreamWriter(manifestFile);
    foreach (var tagFile in _tagFiles)
    {
      var filePath = Path.Combine(bagitDir, tagFile);
      if (!File.Exists(filePath)) throw new FileNotFoundException(null, filePath);
      await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
      var checksum = ChecksumUtility.ComputeSha512(stream);
      // Note there should be 2 spaces between the checksum and the file path
      await writer.WriteLineAsync($"{checksum}  {tagFile}");
    }
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="bagItDir"></param>
  /// <returns></returns>
  public async Task<bool> VerifyChecksums(string bagItDir)
  {
    var hashes = new List<string>();
    var paths = new List<string>();

    // read manifest-sha512.txt
    foreach (var line in await File.ReadAllLinesAsync(Path.Combine(bagItDir, _manifestName)))
    {
      var splitLine = line.Split("  ");
      hashes.Add(splitLine.First());
      paths.Add(splitLine.Last());
    }

    // read tagmanifest-sha512.txt
    foreach (var line in await File.ReadAllLinesAsync(Path.Combine(bagItDir, _tagManifestName)))
    {
      var splitLine = line.Split("  ");
      hashes.Add(splitLine.First());
      paths.Add(splitLine.Last());
    }

    // check equality of hashes to file hashes of file contents
    for (int i = 0; i < paths.Count; i++)
    {
      var fs = new FileStream(
        Path.Combine(bagItDir, paths[i]),
        FileMode.Open,
        FileAccess.Read);
      var checksum = ChecksumUtility.ComputeSha512(fs);
      if (hashes[i] != checksum) return false;
    }

    return true;
  }
}
