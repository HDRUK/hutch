using System.Text.Json.Nodes;

namespace ROCrates.Tests;

public class TestMetadata
{
  private static string _fixtureFileName = "Fixtures/test-entity.json";
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
    var fileJson =
      "{\"@id\": \"cp7glop.ai\", \"@type\": \"File\", \"name\": \"Diagram showing trend to increase\", \"contentSize\": \"383766\", \"description\": \"Illustrator file for Glop Pot\", \"encodingFormat\": \"application/pdf\"}";
    var fileProperties = JsonObject.Parse(fileJson).AsObject();

    var datasetJson =
      "{\"@id\": \"lots_of_little_files/\", \"@type\": \"Dataset\", \"name\": \"Too many files\", \"description\": \"This directory contains many small files, that we're not going to describe in detail.\"}";
    var datasetProperties = JsonObject.Parse(datasetJson).AsObject();

    var file = new Models.File(crate: _roCrate, identifier: fileProperties["@id"].ToString(),
      properties: fileProperties);
    var dataset = new Models.Dataset(crate: _roCrate, identifier: datasetProperties["@id"].ToString(),
      properties: datasetProperties);

    var metadataBasePath = "./";
    var metadataFileName = Path.Combine(metadataBasePath, "ro-crate-metadata.json");
    _roCrate.Add(file, dataset);

    // Act
    _roCrate.Metadata.Write(metadataBasePath);

    var actualOutput = File.ReadAllText(metadataFileName).Trim();

    // Assert
    Assert.Equal(_jsonLd, actualOutput);
  }
}
