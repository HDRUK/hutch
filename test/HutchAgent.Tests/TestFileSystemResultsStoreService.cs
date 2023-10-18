using HutchAgent.Config;
using HutchAgent.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace HutchAgent.Tests;

public class TestFileSystemResultsStoreService
{
  [Fact]
  public void StoreExists_Returns_TrueWhenLocationExists()
  {
    // Arrange
    var existentDirName = Guid.NewGuid().ToString();
    var nonExistentDirName = Guid.NewGuid().ToString();
    Directory.CreateDirectory(existentDirName);

    var logger = new Mock<ILogger<FileSystemResultsStoreService>>();

    var optionsExistent = Options.Create(new FileSystemResultsStoreOptions() { Path = existentDirName });
    var serviceExistent = new FileSystemResultsStoreService(optionsExistent, logger.Object);

    var optionsNonExistent = Options.Create(new FileSystemResultsStoreOptions() { Path = nonExistentDirName });
    var serviceNonExistent = new FileSystemResultsStoreService(optionsNonExistent, logger.Object);

    // Act
    var expectedForTrue = serviceExistent.StoreExists().Result;
    var expectedForFalse = serviceNonExistent.StoreExists().Result;

    // Assert
    Assert.True(expectedForTrue);
    Assert.False(expectedForFalse);

    // Clean up
    if (Directory.Exists(existentDirName)) Directory.Delete(existentDirName);
  }

  [Fact]
  public void WriteToStore_Throws_WhenStoreNonExistent()
  {
    // Arrange
    var nonExistentDirName = Guid.NewGuid().ToString();
    var resultToUpload = new FileInfo($"{Guid.NewGuid().ToString()}.zip");
    resultToUpload.Create().Close();

    var logger = new Mock<ILogger<FileSystemResultsStoreService>>();

    var options = Options.Create(new FileSystemResultsStoreOptions() { Path = nonExistentDirName });
    var service = new FileSystemResultsStoreService(options, logger.Object);

    // Act
    var action = () => service.WriteToStore(resultToUpload.ToString());

    // Assert
    Assert.ThrowsAsync<DirectoryNotFoundException>(action);

    // Clean up
    if (resultToUpload.Exists) resultToUpload.Delete();
  }

  [Fact]
  public void WriteToStore_Throws_WhenUploadTargetNonExistent()
  {
    // Arrange
    var existentDir = new DirectoryInfo(Guid.NewGuid().ToString());
    existentDir.Create();
    var resultToUpload = new FileInfo($"{Guid.NewGuid().ToString()}.zip");

    var logger = new Mock<ILogger<FileSystemResultsStoreService>>();

    var options = Options.Create(new FileSystemResultsStoreOptions() { Path = existentDir.ToString() });
    var service = new FileSystemResultsStoreService(options, logger.Object);

    // Act
    var action = () => service.WriteToStore(resultToUpload.ToString());

    // Assert
    Assert.ThrowsAsync<FileNotFoundException>(action);

    // Clean up
    if (existentDir.Exists) existentDir.Delete(recursive: true);
  }

  [Fact]
  public void WriteToStore_Write_ToStore()
  {
    // Arrange
    var existentDir = new DirectoryInfo(Guid.NewGuid().ToString());
    existentDir.Create();
    var resultToUpload = new FileInfo($"{Guid.NewGuid().ToString()}.zip");
    resultToUpload.Create().Close();

    var logger = new Mock<ILogger<FileSystemResultsStoreService>>();

    var options = Options.Create(new FileSystemResultsStoreOptions() { Path = existentDir.ToString() });
    var service = new FileSystemResultsStoreService(options, logger.Object);

    // Act
    var action = () => service.WriteToStore(resultToUpload.ToString());

    // Assert
    Assert.ThrowsAsync<FileNotFoundException>(action);

    // Clean up
    if (existentDir.Exists) existentDir.Delete(recursive: true);
    if (resultToUpload.Exists) resultToUpload.Delete();
  }

  [Fact]
  public void ResultExists_True_WhenResultInStore()
  {
    // Arrange
    var existentDir = new DirectoryInfo(Guid.NewGuid().ToString());
    existentDir.Create();
    var existentResult = new FileInfo(Path.Combine(existentDir.ToString(), $"{Guid.NewGuid().ToString()}.zip"));
    existentResult.Create().Close();
    var nonExistentResult = new FileInfo(Path.Combine(existentDir.ToString(), $"{Guid.NewGuid().ToString()}.zip"));

    var logger = new Mock<ILogger<FileSystemResultsStoreService>>();

    var options = Options.Create(new FileSystemResultsStoreOptions() { Path = existentDir.ToString() });
    var service = new FileSystemResultsStoreService(options, logger.Object);

    // Act
    var expectedForTrue = service.ResultExists(existentResult.ToString()).Result;
    var expectedForFalse = service.ResultExists(nonExistentResult.ToString()).Result;

    // Assert
    Assert.True(expectedForTrue);
    Assert.False(expectedForFalse);

    // Clean up
    if (existentDir.Exists) existentDir.Delete(recursive: true);
    if (existentResult.Exists) existentResult.Delete();
  }
}
