using System.Text.Json.Nodes;

namespace ROCrates.Tests;

public class TestDataset : IClassFixture<TestDatasetFixture>
{
  private TestDatasetFixture _testDatasetFixture;
  private const string _testDirName = "my-test-dir/";
  private readonly string _testFileJsonFile = "Fixtures/test-dataset.json";

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
      new ROCrate(),
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
      new ROCrate(),
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
      new ROCrate(),
      source: Path.Combine("non", "existent"),
      destPath: testDestPath);

    // Act
    var throwingFunc = () => dataset.Write(_testDatasetFixture.TestBasePath);

    // Assert
    Assert.Throws<DirectoryNotFoundException>(throwingFunc);
  }

  [Fact]
  public void TestDataset_Serialises_Correctly()
  {
    // Arrange
    var expectedJson = File.ReadAllText(_testFileJsonFile).TrimEnd();
    var jsonObject = JsonNode.Parse(expectedJson).AsObject();

    var dataset = new Models.Dataset(
      new ROCrate(),
      properties: jsonObject);

    // Act
    var actualJson = dataset.Serialize();

    // Assert
    Assert.Equal(expectedJson, actualJson);
  }

  [Fact]
  public void DatasetIdTag_Is_UnixPath()
  {
    // Arrange

    // Act

    // Assert
  }

  [Fact]
  public void DatasetIdTag_Ends_WithSlash()
  {
    // Arrange

    // Act

    // Assert
  }

  [Fact]
  public void DatasetIdTag_Is_WholeSpecifiedPath()
  {
    // Arrange

    // Act

    // Assert
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
