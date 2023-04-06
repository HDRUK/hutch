using System.Text.Json.Nodes;

namespace ROCrates.Tests;

public class TestMetadata
{
  private static string _fixtureFileName = "Fixtures/metadata-test.json";
  private ROCrate _roCrate = new();
  private string _jsonLd;

  public TestMetadata()
  {
    _jsonLd = File.ReadAllText(_fixtureFileName).Trim();
  }

  [Fact]
  public void TestMetadata_Writes_Correct_JsonString()
  {
    // Arrange
    var fileJson = File.ReadAllText("Fixtures/test-file.json");
    var fileProperties = JsonNode.Parse(fileJson).AsObject();

    var datasetJson = File.ReadAllText("Fixtures/test-dataset.json");
    var datasetProperties = JsonNode.Parse(datasetJson).AsObject();

    var rootJson = File.ReadAllText("Fixtures/test-root-dataset.json");
    var rootProperties = JsonNode.Parse(rootJson).AsObject();

    var file = new Models.File(crate: _roCrate, identifier: fileProperties["@id"].ToString(),
      properties: fileProperties, source: fileProperties["@id"].ToString());
    var dataset = new Models.Dataset(crate: _roCrate, identifier: datasetProperties["@id"].ToString(),
      properties: datasetProperties, source: datasetProperties["@id"].ToString());
    var root = new Models.RootDataset(crate: _roCrate, identifier: rootProperties["@id"].ToString(),
      properties: rootProperties);

    var metadataBasePath = "./";
    var metadataFileName = Path.Combine(metadataBasePath, "ro-crate-metadata.json");
    _roCrate.Add(root, file, dataset);

    // Act
    _roCrate.Metadata.Write(metadataBasePath);

    var actualOutput = File.ReadAllText(metadataFileName).Trim();

    // Assert
    Assert.Equal(_jsonLd, actualOutput);
  }
}
