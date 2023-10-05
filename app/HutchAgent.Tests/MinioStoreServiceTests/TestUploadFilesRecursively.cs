using HutchAgent.Services;
using Microsoft.Extensions.Logging;
using Minio;
using Moq;

namespace HutchAgent.Tests.MinioStoreServiceTests;

public class TestUploadFilesRecursively : IClassFixture<UploadFilesRecursivelyFixture>
{
  private readonly UploadFilesRecursivelyFixture _fixture;
  private readonly Mock<ILogger<MinioStoreService>> _logger = new();
  private readonly Mock<IMinioClient> _minio = new(); // TODO

  public TestUploadFilesRecursively(UploadFilesRecursivelyFixture fixture)
  {
    _fixture = fixture;
    
    _minio
      .Setup(c => c.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), default))
      .ReturnsAsync(true);
    _minio.Setup(c => c.PutObjectAsync(It.IsAny<PutObjectArgs>(), default));
  }

  [Fact]
  public async void UploadFilesRecursively_WithSingleFile_Throws()
  {
    // Arrange
    var sut = new MinioStoreService(_logger.Object, new(), _minio.Object);

    // Act / Assert
    await Assert.ThrowsAsync<ArgumentException>(
      async () => await sut.UploadFilesRecursively(
        Path.Combine(_fixture.BaseUploadDirectory, _fixture.BaseFile)));
  }
  
  [Fact]
  public async void UploadFilesRecursively_WithDirectory_ListsAllFiles()
  {
    // Arrange
    var sut = new MinioStoreService(_logger.Object, new(), _minio.Object);

    // Act
    var result = await sut.UploadFilesRecursively(Path.Combine(_fixture.BaseUploadDirectory));

    // Assert
    Assert.Equal(4, result.Count);
  }
  
  [Fact]
  public async void UploadFilesRecursively_WithDirectory_PreservesRelativeFilePathsOnly()
  {
    // Arrange
    var sut = new MinioStoreService(_logger.Object, new(), _minio.Object);

    // Act
    var result = await sut.UploadFilesRecursively(Path.Combine(_fixture.BaseUploadDirectory));

    // Assert
    Assert.Contains(_fixture.BaseFile, result);
    Assert.Contains(Path.Combine(_fixture.SubDirectory1, _fixture.File1), result);
    Assert.Contains(Path.Combine(_fixture.SubDirectory2, _fixture.File2), result);
    Assert.Contains(Path.Combine(_fixture.SubDirectory1, _fixture.SubDirectory3, _fixture.File3), result);
  }
  
  [Fact]
  public async void UploadFilesRecursively_WithTargetPrefix_PrependsPrefixToFiles()
  {
    // Arrange
    var prefix = "test";
    var sut = new MinioStoreService(_logger.Object, new(), _minio.Object);

    // Act
    var result = await sut.UploadFilesRecursively(Path.Combine(_fixture.BaseUploadDirectory), prefix);

    // Assert
    Assert.Equal(4, result.Count);
    Assert.Contains(Path.Combine(prefix, _fixture.BaseFile), result);
    Assert.Contains(Path.Combine(prefix, _fixture.SubDirectory1, _fixture.File1), result);
    Assert.Contains(Path.Combine(prefix, _fixture.SubDirectory2, _fixture.File2), result);
    Assert.Contains(Path.Combine(prefix, _fixture.SubDirectory1, _fixture.SubDirectory3, _fixture.File3), result);
  }
}

/// <summary>
/// We need some dummy stuff on disk to "upload"
/// </summary>
public class UploadFilesRecursivelyFixture
{
  public string BaseUploadDirectory { get; } = Guid.NewGuid().ToString();
  public string BaseFile { get; } = "file.json";
  public string SubDirectory1 { get; } = Guid.NewGuid().ToString();
  public string File1 { get; } = "file1.txt";
  public string SubDirectory2 { get; } = Guid.NewGuid().ToString();
  public string File2 { get; } = "file2.md";
  public string SubDirectory3 { get; } = Guid.NewGuid().ToString();
  public string File3 { get; } = "file3.yaml";
  
  public UploadFilesRecursivelyFixture()
  {
    // setup directories and files in the following structure:
    // base/
    // ├─ sub1/
    // │  ├─ sub3/
    // │  │  ├─ file3.yaml
    // │  ├─ file1.txt
    // ├─ sub2/
    // │  ├─ file2.md
    // file.json

    Directory.CreateDirectory(BaseUploadDirectory);
    Directory.CreateDirectory(Path.Combine(BaseUploadDirectory, SubDirectory1));
    Directory.CreateDirectory(Path.Combine(BaseUploadDirectory, SubDirectory2));
    Directory.CreateDirectory(Path.Combine(BaseUploadDirectory, SubDirectory1, SubDirectory3));
    File.Create(Path.Combine(BaseUploadDirectory, BaseFile));
    File.Create(Path.Combine(BaseUploadDirectory, SubDirectory1, File1));
    File.Create(Path.Combine(BaseUploadDirectory, SubDirectory2, File2));
    File.Create(Path.Combine(BaseUploadDirectory, SubDirectory1, SubDirectory3, File3));
  }

  public void Dispose()
  {
    if (Directory.Exists(BaseUploadDirectory)) Directory.Delete(BaseUploadDirectory, recursive: true);
  }
}
