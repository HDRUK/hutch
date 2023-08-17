namespace HutchAgent.Services;

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
      var checksum = _sha512ChecksumService.ComputeSha512(stream);
      // Note there should be 2 spaces between the checksum and the file path
      await writer.WriteLineAsync($"{checksum}  {tagFile}");
    }
  }
}
