namespace ROCrates.Tests;

public class TestPreview : IClassFixture<TestPreviewFixture>
{
  private readonly TestPreviewFixture _testPreviewFixture;

  public TestPreview(TestPreviewFixture testPreviewFixture)
  {
    _testPreviewFixture = testPreviewFixture;
  }

  [Fact]
  public void TestWrite_Creates_PreviewFile()
  {
    // Arrange
    var roCrate = new ROCrate();

    // Act
    roCrate.Preview.Write(Path.GetDirectoryName(_testPreviewFixture.OutputPath));

    // Assert
    Assert.True(System.IO.File.Exists(_testPreviewFixture.OutputPath));
  }
}

public class TestPreviewFixture : IDisposable
{
  public readonly string OutputPath = Path.Combine("./", "ro-crate-preview.html");

  public void Dispose()
  {
    if (System.IO.File.Exists(OutputPath)) System.IO.File.Delete(OutputPath);
  }
}
