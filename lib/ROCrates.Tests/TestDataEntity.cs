using ROCrates.Models;
using Xunit.Abstractions;

namespace ROCrates.Tests;

public class TestDataEntity
{
  private readonly ITestOutputHelper _testOutputHelper;

  public TestDataEntity(ITestOutputHelper testOutputHelper)
  {
    _testOutputHelper = testOutputHelper;
  }

  [Fact]
  public void Test_ConstructorThrows_When_DestPathIsAbsolute()
  {
    Assert.Throws<Exception>(() => new DataEntity(
      new ROCrate("my-test.zip"),
      "",
      null,
      destPath: "/path/to/dest"));
  }

  [Fact]
  public void Test_Identifier_DefaultsTo_DotSlash()
  {
    var dataEntity = new DataEntity(
      new ROCrate("my-test.zip"),
      "",
      null);
    Assert.Equal("./", dataEntity.Identifier);
  }
  
  [Fact]
  public void Test_Identifier_Is_FileNameOnly()
  {
    // relative source
    var dataEntity = new DataEntity(
      new ROCrate("my-test.zip"),
      "",
      null,
      source: "./path/to/my-file.txt");
    Assert.Equal("my-file", dataEntity.Identifier);
    
    // remote source
    dataEntity = new DataEntity(
      new ROCrate("my-test.zip"),
      "",
      null,
      source: "ftp:///path/to/my-file.txt",
      fetchRemote: true);
    Assert.Equal("my-file", dataEntity.Identifier);
  }
}
