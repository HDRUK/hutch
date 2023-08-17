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
}

public class ManifestFixture : IDisposable
{
  private readonly DirectoryInfo _dir = new(Guid.NewGuid().ToString());
  private const string _dataDir = "data";
  private const string _manifestFile = "manifest-sha512.txt";
  private static string[] contents = { "hello world", "foo bar" };

  public string ManifestPath => Path.Combine(_dir.FullName, _manifestFile);
  public DirectoryInfo Dir => _dir;

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
