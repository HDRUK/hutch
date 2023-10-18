using HutchAgent.Services;

namespace HutchAgent.Tests;

public class TestBagItService : IClassFixture<ManifestFixture>, IClassFixture<TagManifestFixture>
{
  private readonly ManifestFixture _manifestFixture;
  private readonly TagManifestFixture _tagManifestFixture;
  private readonly string _testBagitPath = "TestBagit";

  public TestBagItService(ManifestFixture manifestFixture, TagManifestFixture tagManifestFixture)
  {
    _manifestFixture = manifestFixture;
    _tagManifestFixture = tagManifestFixture;
  }

  [Fact]
  public async Task WriteManifestSha512_Creates_ManifestFile()
  {
    // Arrange
    var service = new BagItService();

    // Act
    await service.WriteManifestSha512(_manifestFixture.Dir.FullName);

    // Assert
    Assert.True(File.Exists(_manifestFixture.ManifestPath));
  }

  [Fact]
  public async Task WriteManifestSha512_Writes_CorrectChecksums()
  {
    // Arrange
    var service = new BagItService();

    // Act
    await service.WriteManifestSha512(_manifestFixture.Dir.FullName);
    var lines = await File.ReadAllLinesAsync(_manifestFixture.ManifestPath);
    var hashes = from x in lines select x.Split("  ").First();

    // Assert
    foreach (var h in hashes.ToArray())
    {
      Assert.Contains(h, _manifestFixture.ExpectedHashes);
    }
  }

  [Fact]
  public async Task WriteManifestSha512_Writes_CorrectFilePaths()
  {
    // Arrange
    var service = new BagItService();

    // Act
    await service.WriteManifestSha512(_manifestFixture.Dir.FullName);
    var lines = await File.ReadAllLinesAsync(_manifestFixture.ManifestPath);
    var paths = from x in lines select x.Split("  ").Last();

    // Assert
    foreach (var p in paths.ToArray())
    {
      Assert.Contains(p, _manifestFixture.ExpectedPaths);
    }
  }

  [Fact]
  public async Task WriteTagManifestSha512_Creates_TagManifestFile()
  {
    // Arrange
    var service = new BagItService();

    // Act
    await service.WriteTagManifestSha512(_tagManifestFixture.Dir.FullName);

    // Assert
    Assert.True(File.Exists(_tagManifestFixture.TagManifestPath));
  }

  [Fact]
  public async Task WriteTagManifestSha512_Writes_CorrectChecksums()
  {
    // Arrange
    var service = new BagItService();

    // Act
    await service.WriteTagManifestSha512(_tagManifestFixture.Dir.FullName);
    var lines = await File.ReadAllLinesAsync(_tagManifestFixture.TagManifestPath);
    var hashes = from x in lines select x.Split("  ").First();

    // Assert
    foreach (var h in hashes.ToArray())
    {
      Assert.Contains(h, _tagManifestFixture.ExpectedHashes);
    }
  }

  [Fact]
  public async Task WriteTagManifestSha512_Writes_CorrectFilePaths()
  {
    // Arrange
    var service = new BagItService();

    // Act
    await service.WriteTagManifestSha512(_tagManifestFixture.Dir.FullName);
    var lines = await File.ReadAllLinesAsync(_tagManifestFixture.TagManifestPath);
    var paths = from x in lines select x.Split("  ").Last();

    // Assert
    foreach (var p in paths.ToArray())
    {
      Assert.Contains(p, _tagManifestFixture.ExpectedPaths);
    }
  }

  [Fact]
  public async Task VerifyChecksum_Returns_TrueWhenChecksumsAreCorrect()
  {
    // Arrange
    var service = new BagItService();

    // Act
    var valid = await service.VerifyChecksums(_testBagitPath);

    // Assert
    Assert.True(valid);
  }
}

public class ManifestFixture : IDisposable
{
  private readonly DirectoryInfo _dir = new(Guid.NewGuid().ToString());
  private const string _dataDir = "data";
  private const string _manifestFile = "manifest-sha512.txt";
  private static string[] contents = { "hello world", "foo bar" };

  public string ManifestPath => Path.Combine(_dir.FullName, _manifestFile);
  public DirectoryInfo Dir => _dir;

  public string[] ExpectedHashes => new[]
  {
    "309ecc489c12d6eb4cc40f50c902f2b4d0ed77ee511a7c7a9bcd3ca86d4cd86f989dd35bc5ff499670da34255b45b0cfd830e81f605dcf7dc5542e93ae9cd76f",
    "65019286222ace418f742556366f9b9da5aaf6797527d2f0cba5bfe6b2f8ed24746542a0f2be1da8d63c2477f688b608eb53628993afa624f378b03f10090ce7"
  };

  public List<string> ExpectedPaths = new();

  public ManifestFixture()
  {
    _dir.Create();
    _dir.CreateSubdirectory(_dataDir);
    for (var i = 0; i < contents.Length; i++)
    {
      var filePath = Path.Combine(_dir.FullName, _dataDir, $"{i}.txt");
      using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
      using var writer = new StreamWriter(stream);
      writer.Write(contents[i]);
      ExpectedPaths.Add(Path.GetRelativePath(_dir.FullName, filePath));
    }
  }

  public void Dispose()
  {
    if (_dir.Exists) _dir.Delete(recursive: true);
  }
}

public class TagManifestFixture : IDisposable
{
  private readonly DirectoryInfo _dir = new(Guid.NewGuid().ToString());
  private const string _tagManifestFile = "tagmanifest-sha512.txt";
  private static string[] contents = { "hello world", "foo bar", "baz" };

  private static string[] _tagFiles =
    { "bagit.txt", "bagit-info.txt", "manifest-sha512.txt" };

  public string TagManifestPath => Path.Combine(_dir.FullName, _tagManifestFile);
  public DirectoryInfo Dir => _dir;

  public string[] ExpectedHashes => new[]
  {
    "309ecc489c12d6eb4cc40f50c902f2b4d0ed77ee511a7c7a9bcd3ca86d4cd86f989dd35bc5ff499670da34255b45b0cfd830e81f605dcf7dc5542e93ae9cd76f",
    "65019286222ace418f742556366f9b9da5aaf6797527d2f0cba5bfe6b2f8ed24746542a0f2be1da8d63c2477f688b608eb53628993afa624f378b03f10090ce7",
    "22b41602570746d784cef124fa6713eec180f93af02a1bfee05528e94a1b053e4136b446015161d04e9900849575bd8f95f857773868a205dbed42413cd054f1"
  };

  public List<string> ExpectedPaths = new();

  public TagManifestFixture()
  {
    _dir.Create();
    for (var i = 0; i < contents.Length; i++)
    {
      var filePath = Path.Combine(_dir.FullName, _tagFiles[i]);
      using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
      using var writer = new StreamWriter(stream);
      writer.Write(contents[i]);
      ExpectedPaths.Add(_tagFiles[i]);
    }
  }

  public void Dispose()
  {
    if (_dir.Exists) _dir.Delete(recursive: true);
  }
}
