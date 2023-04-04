using System.Text.Json.Nodes;
using ROCrates.Models;
using File = System.IO.File;

namespace ROCrates.Tests;

public class TestComputerLanguage
{
  private readonly string _testFileJsonFile = "Fixtures/test-computer-language.json";

  [Fact]
  public void TestDefaultType_Is_ComputerLanguage()
  {
    // Arrange
    var roCrate = new ROCrate();
    var computerLanguage = new ComputerLanguage(roCrate);
    const string expectedType = "ComputerLanguage";

    // Act
    var actualType = computerLanguage.GetProperty<string>("@type");

    // Assert
    Assert.NotNull(actualType);
    Assert.Equal(expectedType, actualType);
  }

  [Fact]
  public void TestComputerLanguage_Serialises_Correctly()
  {
    // Arrange
    var expectedJson = File.ReadAllText(_testFileJsonFile).TrimEnd();
    var jsonObject = JsonNode.Parse(expectedJson).AsObject();

    var computerLanguage = new ComputerLanguage(new ROCrate(),
      identifier: jsonObject["@id"].ToString(),
      properties: jsonObject
    );

    // Act
    var actualJson = computerLanguage.Serialize();

    // Assert
    Assert.Equal(expectedJson, actualJson);
  }
}
