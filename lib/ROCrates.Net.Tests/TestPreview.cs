using System.Text.Json.Nodes;
using ROCrates.Models;

namespace ROCrates.Tests;

public class TestPreview
{
  private ROCrate _roCrate = new();

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

    var previewBasePath = "./";
    var previewFileName = Path.Combine(previewBasePath, "ro-crate-preview.html");
    _roCrate.Add(rootDataset, preview);

    // Act
    preview.Write(previewBasePath);

    // Assert
    Assert.True(System.IO.File.Exists(previewFileName));
  }
}
