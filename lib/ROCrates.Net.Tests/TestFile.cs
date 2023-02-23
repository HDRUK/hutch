using Xunit.Abstractions;
using Xunit.Sdk;

namespace ROCrates.Tests;

public class TestFile : IDisposable
{
  private readonly ITestOutputHelper _testOutputHelper;
  private static char _sep = Path.DirectorySeparatorChar;
  private const string _testFileName = "my-test-file.txt";
  private static readonly string _testBasePath = $"path{_sep}to{_sep}base";

  public TestFile(ITestOutputHelper testOutputHelper)
  {
    _testOutputHelper = testOutputHelper;
    Directory.CreateDirectory(_testBasePath);
    using var file = new StreamWriter(Path.Combine(_testBasePath, _testFileName));
    file.WriteLine("some content");
  }

  public void Dispose()
  {
    File.Delete(_testFileName);
    var rootDir = _testBasePath.Split(Path.DirectorySeparatorChar).First();
    Directory.Delete(rootDir, recursive: true);
  }
  
  [Fact]
  public void TestWrite_Saves_To_DestPath()
  {
    var testDestPath = Path.Combine(_testBasePath, "ext", _testFileName);
    var fileEntity = new Models.File(
      new ROCrate(_testFileName),
      source: Path.Combine(_testBasePath, _testFileName),
      destPath: testDestPath);
    fileEntity.Write(_testBasePath);
    Assert.True(File.Exists(Path.Combine(_testBasePath, testDestPath)));
  }
  
  [Fact]
  public void TestWrite_Saves_To_Source()
  {
    var fileEntity = new Models.File(
      new ROCrate(_testFileName),
      source: Path.Combine(_testBasePath, _testFileName));
    fileEntity.Write(_testBasePath);
    Assert.True(File.Exists(Path.Combine(_testBasePath, _testFileName)));
  }
  
  [Fact]
  public void TestWrite_Saves_From_Remote()
  {
    var url = "https://hdruk.github.io/hutch/docs/devs";
    var fileName = url.Split('/').Last();
    var fileEntity = new Models.File(
      new ROCrate(_testFileName),
      source: url,
      validateUrl: true,
      fetchRemote: true);
    fileEntity.Write(_testBasePath);
    Assert.True(File.Exists(Path.Combine(_testBasePath, fileName)));
  }
}
