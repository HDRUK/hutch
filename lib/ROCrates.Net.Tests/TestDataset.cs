namespace ROCrates.Tests;

public class TestDataset : IClassFixture<TestDatasetFixture>
{
  private TestDatasetFixture _testDatasetFixture;
  private const string _testDirName = "my-test-dir/";

  public TestDataset(TestDatasetFixture testDatasetFixture)
  {
    _testDatasetFixture = testDatasetFixture;
  }

  [Fact]
  public void TestWrite_Creates_Dir_From_Source()
  {
    // Arrange
    var sourceDir = Path.Combine(_testDatasetFixture.TestBasePath, _testDirName);
    Directory.CreateDirectory(sourceDir);
    var dataset = new Models.Dataset(
      new ROCrate(_testDirName),
      source: Path.Combine(_testDatasetFixture.TestBasePath, _testDirName));

    // Act
    dataset.Write(_testDatasetFixture.TestBasePath);

    // Assert
    Assert.True(Directory.Exists(Path.Combine(_testDatasetFixture.TestBasePath, sourceDir)));
  }

  [Fact]
  public void TestWrite_Creates_Dir_From_DestPath()
  {
    // Arrange
    var sourceDir = Path.Combine(_testDatasetFixture.TestBasePath, _testDirName);
    var destPath = Path.Combine(_testDatasetFixture.TestBasePath, "ext", _testDirName);
    Directory.CreateDirectory(sourceDir);
    Directory.CreateDirectory(destPath);
    var dataset = new Models.Dataset(
      new ROCrate(_testDirName),
      source: Path.Combine(_testDatasetFixture.TestBasePath, _testDirName),
      destPath: destPath);

    // Act
    dataset.Write(_testDatasetFixture.TestBasePath);

    // Assert
    Assert.True(Directory.Exists(Path.Combine(_testDatasetFixture.TestBasePath, destPath)));
  }

  [Fact]
  public void TestWrite_Throws_When_SourceDir_DoesNotExist()
  {
    // Arrange
    var testDestPath = Path.Combine(_testDatasetFixture.TestBasePath, "ext", _testDirName);
    Directory.CreateDirectory(testDestPath);
    var dataset = new Models.Dataset(
      new ROCrate(_testDirName),
      source: Path.Combine("non", "existent"),
      destPath: testDestPath);

    // Act
    var throwingFunc = () => dataset.Write(_testDatasetFixture.TestBasePath);

    // Assert
    Assert.Throws<DirectoryNotFoundException>(throwingFunc);
  }
}

public class TestDatasetFixture : IDisposable
{
  public readonly string TestBasePath = Path.Combine("dataset", "test", "path");

  public TestDatasetFixture()
  {
    Directory.CreateDirectory(TestBasePath);
  }

  public void Dispose()
  {
    Directory.Delete(TestBasePath, recursive: true);
  }
}
