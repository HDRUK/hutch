using System.Text.Json.Nodes;
using ROCrates.Models;
using File = System.IO.File;

namespace ROCrates.Tests;

public class TestComputationalWorkflow
{
  private readonly string _testFileJsonFile = "Fixtures/test-computational-workflow.json";

  [Fact]
  public void TestComputationalWorkflow_Serialises_Correctly()
  {
    // Arrange
    var expectedJson = File.ReadAllText(_testFileJsonFile).TrimEnd();
    var jsonObject = JsonNode.Parse(expectedJson).AsObject();

    var computationalWorkflow = new ComputationalWorkflow(new ROCrate(),
      identifier: jsonObject["@id"].ToString(),
      properties: jsonObject
    );

    // Act
    var actualJson = computationalWorkflow.Serialize();

    // Assert
    Assert.Equal(expectedJson, actualJson);
  }
}
