using HutchAgent.Services;
using Moq;

namespace HutchAgent.Tests;

public class TestBagitChecksumWriter : IClassFixture<ManifestFixture>
{
  private ManifestFixture _manifestFixture;
  private Mock<IServiceProvider> _serviceProvider = new();

  public TestBagitChecksumWriter(ManifestFixture manifestFixture)
  {
    _manifestFixture = manifestFixture;
    _serviceProvider
      .Setup(m => m.GetService(typeof(Sha512ChecksumService)))
      .Returns(new Sha512ChecksumService());
  }

  [Fact]
  public async Task WriteManifestSha512_Creates_ManifestFile()
  {
    // Arrange
    var service = new BagitChecksumWriter(_serviceProvider.Object);

    // Act
    await service.WriteManifestSha512(_manifestFixture.Dir.FullName);

    // Assert
    Assert.True(File.Exists(_manifestFixture.ManifestPath));
  }

  [Fact]
  public async Task WriteManifestSha512_Writes_CorrectChecksums()
  {
    // Arrange
    var service = new BagitChecksumWriter(_serviceProvider.Object);

    // Act
    await service.WriteManifestSha512(_manifestFixture.Dir.FullName);
    var lines = await File.ReadAllLinesAsync(_manifestFixture.ManifestPath);
    var hashes = from x in lines select x.Split("  ").First();

    // Assert
    Assert.Equal(_manifestFixture.ExpectedHashes, hashes.ToArray());
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
      writer.Write(contents[i]);
    }
  }

  public void Dispose()
  {
    if (_dir.Exists) _dir.Delete(recursive: true);
  }
}
