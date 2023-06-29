using System.Text.Json.Nodes;

namespace ROCrates.Tests;

public class TestMetadata
{
  private ROCrate _roCrate = new();

  [Fact]
  public void TestMetadata_Writes_Correct_JsonString()
  {
    // Arrange
    var fileJson = File.ReadAllText("Fixtures/test-file.json");
    var fileProperties = JsonNode.Parse(fileJson).AsObject();

    var datasetJson = File.ReadAllText("Fixtures/test-dataset.json");
    var datasetProperties = JsonNode.Parse(datasetJson).AsObject();

    var file = new Models.File(crate: _roCrate, identifier: fileProperties["@id"].ToString(),
      properties: fileProperties, source: fileProperties["@id"].ToString());
    var dataset = new Models.Dataset(crate: _roCrate, identifier: datasetProperties["@id"].ToString(),
      properties: datasetProperties, source: datasetProperties["@id"].ToString());

    var metadataBasePath = "./";
    var metadataFileName = Path.Combine(metadataBasePath, "ro-crate-metadata.json");
    _roCrate.Add(file, dataset);

    // Act
    _roCrate.Metadata.Write(metadataBasePath);

    var actualOutput = File.ReadAllText(metadataFileName).Trim();
    var json = JsonNode.Parse(actualOutput);
    json!.AsObject().TryGetPropertyValue("@graph", out var graph);

    // Assert
    Assert.NotNull(graph);
    foreach (var g in graph!.AsArray())
    {
      Assert.NotNull(g);
      if (g!["@id"]!.ToString() != "./" && g["@id"]!.ToString() != _roCrate.Metadata.Id &&
          g["@id"]!.ToString() != _roCrate.Preview.Id)
      {
        Assert.True(
          g.ToJsonString() == file.Properties.ToJsonString() ||
          g.ToJsonString() == dataset.Properties.ToJsonString()
        );
      }
    }
  }
}
