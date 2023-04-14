using System.Text.Json.Nodes;
using ROCrates.Models;

namespace ROCrates.Tests;

public class TestPreview : IClassFixture<TestPreviewFixture>
{
  private ROCrate _roCrate = new();
  private readonly TestPreviewFixture _testPreviewFixture;

  public TestPreview(TestPreviewFixture testPreviewFixture)
  {
    _testPreviewFixture = testPreviewFixture;
  }

  [Fact]
  public void TestWrite_Creates_PreviewFile()
  {
    // Arrange
    var rootJson = System.IO.File.ReadAllText("Fixtures/test-root-dataset.json").TrimEnd();
    var rootProperties = JsonNode.Parse(rootJson).AsObject();
    var rootDataset = new RootDataset(_roCrate, properties: rootProperties);

    var previewJson = System.IO.File.ReadAllText("Fixtures/test-preview.json").TrimEnd();
    var previewProperties = JsonNode.Parse(previewJson).AsObject();
    var preview = new Preview(_roCrate, properties: previewProperties);


    _roCrate.Add(rootDataset, preview);

    // Act
    preview.Write(Path.GetDirectoryName(_testPreviewFixture.OutputPath));

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
