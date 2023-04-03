using System.Text.Json.Nodes;

namespace ROCrates.Tests;

public class TestPerson
{
  private readonly string _testFileJsonFile = "Fixtures/test-person.json";

  [Fact]
  public void TestFile_Serialises_Correctly()
  {
    // Arrange
    var expectedJson = File.ReadAllText(_testFileJsonFile).TrimEnd();
    var jsonObject = JsonNode.Parse(expectedJson).AsObject();

    var person = new Models.Person(new ROCrate(),
      identifier: jsonObject["@id"].ToString(),
      properties: jsonObject
    );

    // Act
    var actualJson = person.Serialize();

    // Assert
    Assert.Equal(expectedJson, actualJson);
  }
}
