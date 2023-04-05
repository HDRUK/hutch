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
      new ROCrate("my-test.zip"),
      destPath: "/path/to/dest"));
  }

  [Fact]
  public void Test_Identifier_DefaultsTo_DotSlash()
  {
    var dataEntity = new FileOrDir(
      new ROCrate("my-test.zip"));
    Assert.Equal("./", dataEntity.Id);
  }

  [Fact]
  public void Test_Identifier_Is_FileName_With_Extension()
  {
    // Arrange
    var localName = "./path/to/my-file.txt";
    var remaoteName = "ftp:///path/to/my-file.txt";

    // Act
    var dataEntityWithLocal = new FileOrDir(
      new ROCrate("my-test.zip"),
      source: "./path/to/my-file.txt");
    var dataEntityWithRemote = new FileOrDir(
      new ROCrate("my-test.zip"),
      source: "ftp:///path/to/my-file.txt",
      fetchRemote: true);

    // Assert
    Assert.Equal("my-file.txt", dataEntityWithLocal.Id);
    Assert.Equal("my-file.txt", dataEntityWithRemote.Id);
  }
}
