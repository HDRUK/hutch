using Xunit.Abstractions;

namespace ROCrates.Tests;

public class TestDataset : IClassFixture<TestDatasetFixture>
{
  private readonly ITestOutputHelper _testOutputHelper;
  private TestDatasetFixture _testDatasetFixture;
  private static char _sep = Path.DirectorySeparatorChar;
  private const string _testDirName = "my-test-file/";
  private static readonly string _testBasePath = $"path{_sep}to{_sep}base";
  
  public TestDataset(ITestOutputHelper testOutputHelper, TestDatasetFixture testDatasetFixture)
  {
    _testOutputHelper = testOutputHelper;
    _testDatasetFixture = testDatasetFixture;
  }

  [Fact]
  public void TestWrite_Creates_Dir_From_Source()
  {
    var sourceDir = Path.Combine(_testBasePath, _testDirName);
    Directory.CreateDirectory(sourceDir);
    var dataset = new Models.Dataset(
      new ROCrate(_testDirName),
      source: Path.Combine(_testBasePath, _testDirName));
    dataset.Write(_testBasePath);
    Assert.True(Directory.Exists(Path.Combine(_testBasePath, sourceDir)));
  }
  
  [Fact]
  public void TestWrite_Creates_Dir_From_DestPath()
  {
    var sourceDir = Path.Combine(_testBasePath, _testDirName);
    var destPath = Path.Combine(_testBasePath, "ext", _testDirName);
    Directory.CreateDirectory(sourceDir);
    Directory.CreateDirectory(destPath);
    var dataset = new Models.Dataset(
      new ROCrate(_testDirName),
      source: Path.Combine(_testBasePath, _testDirName),
      destPath: destPath);
    dataset.Write(_testBasePath);
    Assert.True(Directory.Exists(Path.Combine(_testBasePath, destPath)));
  }
  
  [Fact]
  public void TestWrite_Throws_When_SourceDir_DoesNotExist()
  {
    var testDestPath = Path.Combine(_testBasePath, "ext", _testDirName);
    Directory.CreateDirectory(testDestPath);
    var dataset = new Models.Dataset(
      new ROCrate(_testDirName),
      source: Path.Combine("non", "existent"),
      destPath: testDestPath);
    Assert.Throws<DirectoryNotFoundException>(() => dataset.Write(_testBasePath));
  }
}

public class TestDatasetFixture : IDisposable
{
  private static char _sep = Path.DirectorySeparatorChar;
  private static readonly string _testBasePath = $"path{_sep}to{_sep}base";
  
  public TestDatasetFixture()
  {
    Directory.CreateDirectory(_testBasePath);
  }
  public void Dispose()
  {
    Directory.Delete(_testBasePath, recursive: true);
  }
}
