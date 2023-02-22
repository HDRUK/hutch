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
    var pathArray = _testBasePath.Split(Path.DirectorySeparatorChar);
    Directory.Delete(pathArray[0], recursive: true);
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
}
