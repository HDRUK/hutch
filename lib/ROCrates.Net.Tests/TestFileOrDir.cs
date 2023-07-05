using ROCrates.Models;
using Xunit.Abstractions;

namespace ROCrates.Tests;

public class TestFileOrDir
{
  private readonly ITestOutputHelper _testOutputHelper;

  public TestFileOrDir(ITestOutputHelper testOutputHelper)
  {
    _testOutputHelper = testOutputHelper;
  }

  [Fact]
  public void Test_ConstructorThrows_When_DestPathIsAbsolute()
  {
    Assert.Throws<Exception>(() => new FileOrDir(
      new ROCrate(),
      destPath: "/path/to/dest"));
  }

  [Fact]
  public void Test_Identifier_DefaultsTo_DotSlash()
  {
    var dataEntity = new FileOrDir(new ROCrate());
    Assert.Equal("./", dataEntity.Id);
  }

  [Fact]
  public void Test_Identifier_Is_FileName_With_Extension()
  {
    // Arrange
    const string localName = "./path/to/my-file.txt";
    const string remoteName = "ftp:/some.host/path/to/my-file.txt";

    // Act
    var dataEntityWithLocal = new FileOrDir(
      new ROCrate(),
      source: localName);
    var dataEntityWithRemote = new FileOrDir(
      new ROCrate(),
      source: remoteName,
      fetchRemote: true);

    // Assert
    Assert.Equal(localName, dataEntityWithLocal.Id);
    Assert.Equal(remoteName, dataEntityWithRemote.Id);
  }

  [Fact]
  public void Test_Identifier_Is_DirName()
  {
    // Arrange
    var dirName = "./path/to/my-dir/";

    // Act
    var dataEntityWithDir = new FileOrDir(
      new ROCrate(),
      source: dirName);

    // Assert
    Assert.Equal(dirName, dataEntityWithDir.Id);
  }
}
