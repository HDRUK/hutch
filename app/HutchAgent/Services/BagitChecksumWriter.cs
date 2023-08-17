namespace HutchAgent.Services;

/// <summary>
/// Service for creating the checksum files for a Bagit archive.
/// </summary>
public class BagitChecksumWriter
{
  private const string _manifestName = "manifest-sha512.txt";
  private const string _tagManifestName = "tagmanifest-sha512.txt";
  private const string _dataDir = "data";

  private static string[] _tagFiles =
    { "bagit.txt", "bag-info.txt", "manifest-sha512.txt" };

  private readonly Sha512ChecksumService _sha512ChecksumService;

  public BagitChecksumWriter(IServiceProvider serviceProvider)
  {
    _sha512ChecksumService =
      serviceProvider.GetService<Sha512ChecksumService>() ?? throw new InvalidOperationException();
  }

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
    foreach (var entry in Directory.EnumerateFiles(Path.Combine(bagitDir, _dataDir), "*", SearchOption.AllDirectories))
    {
      await using var stream = new FileStream(entry, FileMode.Open, FileAccess.Read);
      var checksum = _sha512ChecksumService.ComputeSha512(stream);
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
  public async Task WriteTagManifestSha512(string bagitDir)
  {
    await using var manifestFile =
      new FileStream(Path.Combine(bagitDir, _tagManifestName), FileMode.Create, FileAccess.Write);
    await using var writer = new StreamWriter(manifestFile);
    foreach (var tagFile in _tagFiles)
    {
      var filePath = Path.Combine(bagitDir, tagFile);
      await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
      var checksum = _sha512ChecksumService.ComputeSha512(stream);
      // Note there should be 2 spaces between the checksum and the file path
      await writer.WriteLineAsync($"{checksum}  {tagFile}");
    }
  }
}
