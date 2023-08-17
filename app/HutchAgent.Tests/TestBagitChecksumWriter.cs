namespace HutchAgent.Tests;

public class TestBagitChecksumWriter
{
}

public class ManifestFixture : IDisposable
{
  private readonly DirectoryInfo _dir = new(Guid.NewGuid().ToString());
  private const string _dataDir = "data";
  private const string _manifestFile = "manifest-sha512.txt";
  private static string[] contents = { "hello world", "foo bar" };

  public string ManifestPath => Path.Combine(_dir.FullName, _manifestFile);

  public ManifestFixture()
  {
    _dir.Create();
    _dir.CreateSubdirectory(_dataDir);
    for (var i = 0; i < contents.Length; i++)
    {
      using var stream = new FileStream(
        Path.Combine(_dir.FullName, _dataDir, $"{i}.txt"),
        FileMode.Create,
        FileAccess.Write);
      using var writer = new StreamWriter(stream);
      writer.WriteLine(contents[i]);
    }
  }

  public void Dispose()
  {
    if (_dir.Exists) _dir.Delete(recursive: true);
  }
}
