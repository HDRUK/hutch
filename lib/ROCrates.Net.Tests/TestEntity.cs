using System.Text.Json;
using System.Text.Json.Nodes;
using ROCrates.Models;
using Xunit.Abstractions;

namespace ROCrates.Tests;

public class TestEntity
{
  private ROCrate _roCrate = new();
  private string _jsonLd =
    "{\"@id\": \"./\",\"identifier\": \"https://doi.org/10.4225/59/59672c09f4a4b\",\"@type\": \"Dataset\", \"randomNumber\": 123, \"datePublished\": \"2017\",\"name\": \"Data files associated with the manuscript:Effects of facilitated family case conferencing for ...\",\"description\": \"Palliative care planning for nursing home residents with advanced dementia ...\",\"license\": {\"@id\": \"https://creativecommons.org/licenses/by-nc-sa/3.0/au/\"}}";

  
  [Fact]
  public void TestSetProperty_Updates()
  {
    var jsonObject = JsonNode.Parse(_jsonLd).AsObject();
    var entity = new Entity(_roCrate, properties: jsonObject);
    var gotValue = entity.Properties.TryGetPropertyValue("datePublished", out var datePublished);
    Assert.True(gotValue);
    Assert.Equal("2017", datePublished.ToString());
    entity.SetProperty("datePublished", "2023");
    entity.Properties.TryGetPropertyValue("datePublished", out datePublished);
    Assert.Equal("2023", datePublished.ToString());
  }

  [Fact]
  public void TestGetProperty_CorrectTypes()
  {
    var jsonObject = JsonNode.Parse(_jsonLd).AsObject();
    var entity = new Entity(_roCrate, properties: jsonObject);
    var retrievedInt = entity.GetProperty<int>("randomNumber");
    Assert.IsType<int>(retrievedInt);
    Assert.Equal(123, retrievedInt);

    var retrievedString = entity.GetProperty<string>("@type");
    Assert.IsType<string>(retrievedString);
    Assert.Equal("Dataset", retrievedString);
  }

  [Fact]
  public void TestDefaultProperties()
  {
    var entity = new Entity(_roCrate);
    var gotValue = entity.Properties.TryGetPropertyValue("@type", out var type);
    Assert.True(gotValue);
    Assert.Equal("Thing", type.ToString());
  }
  
  [Fact]
  public void TestProperties_Type_Is_AsSpecified()
  {
    var jsonObject = JsonNode.Parse(_jsonLd).AsObject();
    var entity = new Entity(
      _roCrate,
      properties: jsonObject);
    var gotValue = entity.Properties.TryGetPropertyValue("@type", out var type);
    Assert.True(gotValue);
    Assert.Equal("Dataset", type.ToString());
  }

  [Fact]
  public void TestAppendTo_CreatesHasPart()
  {
    var jsonObject = JsonNode.Parse(_jsonLd).AsObject();
    var entity = new Entity(_roCrate, properties: jsonObject);
    var entity2 = new Entity(_roCrate);
    entity.AppendTo("hasPart", entity2);
    
    Assert.True(entity.Properties.ContainsKey("hasPart"));
    Assert.True(entity.Properties.TryGetPropertyValue("hasPart", out var outputIdJson));
    var outputId = outputIdJson.Deserialize<Part>();
    Assert.Equal(entity2.GetCanonicalId(), outputId.Identifier);
  }
}
