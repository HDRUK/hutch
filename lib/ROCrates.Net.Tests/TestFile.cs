namespace ROCrates.Tests;

public class TestFile : IClassFixture<TestFileFixture>
{
  private readonly TestFileFixture _testFileFixture;
  private readonly string _testBasePath;

  public TestFile(TestFileFixture testFileFixture)
  {
    _testFileFixture = testFileFixture;
    _testBasePath = testFileFixture.TestBasePath;
  }

  [Fact]
  public void TestWrite_Saves_To_DestPath()
  {
    // Arrange
    var testDestPath = Path.Combine(_testBasePath, "ext", _testFileFixture.TestFileName);
    var fileEntity = new Models.File(
      new ROCrate(_testFileFixture.TestFileName),
      source: Path.Combine(_testBasePath, _testFileFixture.TestFileName),
      destPath: testDestPath);

    // Act
    fileEntity.Write(_testBasePath);

    // Assert
    Assert.True(File.Exists(Path.Combine(_testBasePath, testDestPath)));
  }

  [Fact]
  public void TestWrite_Saves_To_Source()
  {
    // Arrange
    var fileEntity = new Models.File(
      new ROCrate(_testFileFixture.TestFileName),
      source: Path.Combine(_testBasePath, _testFileFixture.TestFileName));

    // Act
    fileEntity.Write(_testBasePath);

    // Assert
    Assert.True(File.Exists(Path.Combine(_testBasePath, _testFileFixture.TestFileName)));
  }

  [Fact]
  public void TestWrite_Saves_From_Remote()
  {
    // Arrange
    var url = "https://hdruk.github.io/hutch/docs/devs";
    var fileName = url.Split('/').Last();
    var fileEntity = new Models.File(
      new ROCrate(_testFileFixture.TestFileName),
      source: url,
      validateUrl: true,
      fetchRemote: true);

    // Act
    fileEntity.Write(_testBasePath);

    // Assert
    Assert.True(File.Exists(Path.Combine(_testBasePath, fileName)));
  }
}

public class TestFileFixture : IDisposable
{
  public readonly string TestFileName = "my-test-file.txt";
  public readonly string TestBasePath = Path.Combine("file", "test", "path");

  public TestFileFixture()
  {
    Directory.CreateDirectory(TestBasePath);
    using var file = new StreamWriter(Path.Combine(TestBasePath, TestFileName));
    file.WriteLine("some content");
  }

  public void Dispose()
  {
    File.Delete(TestFileName);
    var rootDir = TestBasePath.Split(Path.DirectorySeparatorChar).First();
    Directory.Delete(rootDir, recursive: true);
  }
}
