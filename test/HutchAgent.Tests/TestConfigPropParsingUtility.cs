using System.Collections;
using System.Text.Json.Nodes;
using HutchAgent.Utilities;
using Microsoft.Extensions.Configuration;

namespace HutchAgent.Tests;

public class TestConfigPropParsingUtility
{
  private readonly IConfigurationRoot _configurationRoot;

  public TestConfigPropParsingUtility()
  {
    var builder = new ConfigurationBuilder()
      .AddJsonFile("Data/testConfigParse.json");
    _configurationRoot = builder.Build();
  }

  [Theory]
  [ClassData(typeof(ConfigFixture))]
  public void GetObject_Return_ExpectedObject(string sectionPath, JsonObject expectedObject)
  {
    // Arrange
    var utility = new ConfigPropsParsingUtility(_configurationRoot);

    // Act
    var actualObject = utility.GetObject(sectionPath);

    // Assert
    Assert.Equal(expectedObject.ToJsonString(), actualObject.ToJsonString());
  }
}

public class ConfigFixture : IEnumerable<object[]>
{
  public IEnumerator<object[]> GetEnumerator()
  {
    yield return new object[]
    {
      "hello", new JsonObject { ["hello"] = "world" }
    };
    yield return new object[]
    {
      "CratePublishing:Publisher",
      new JsonObject
      {
        ["CratePublishing"] = new JsonObject
          { ["Publisher"] = new JsonObject { ["Id"] = "https://trefx.example.com/" } }
      }
    };
    yield return new object[]
    {
      "CratePublishing:License",
      new JsonObject
      {
        ["CratePublishing"] = new JsonObject
        {
          ["License"] = new JsonObject
          {
            ["Uri"] = "https://spdx.org/licenses/CC-BY-4.0",
            ["Properties"] = new JsonObject
            {
              ["name"] = "Creative Commons Attribution 4.0 International",
              ["identifier"] = "CC-BY-4.0"
            }
          }
        }
      }
    };
    yield return new object[]
    {
      "number", new JsonObject { ["number"] = 1 }
    };
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }
}
