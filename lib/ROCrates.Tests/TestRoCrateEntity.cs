using System.Text.Json;
using System.Text.Json.Nodes;
using ROCrates.Models;

namespace ROCrates.Tests;

public class TestRoCrateEntity
{
    [Fact]
    public void TestSetProperty_Updates()
    {
      var jsonLd = "{\"@id\": \"./\",\"identifier\": \"https://doi.org/10.4225/59/59672c09f4a4b\",\"@type\": \"Dataset\",\"datePublished\": \"2017\",\"name\": \"Data files associated with the manuscript:Effects of facilitated family case conferencing for ...\",\"description\": \"Palliative care planning for nursing home residents with advanced dementia ...\",\"license\": {\"@id\": \"https://creativecommons.org/licenses/by-nc-sa/3.0/au/\"}}";
      var jsonObject = JsonNode.Parse(jsonLd).AsObject();
      var entity = new Entity(new ROCrate(),"", jsonObject);
      var gotValue = entity.Properties.TryGetPropertyValue("datePublished",out var datePublished);
      Assert.True(gotValue);
      Assert.Equal("2017",datePublished.ToString());
      entity.SetProperty("datePublished","2023");
      entity.Properties.TryGetPropertyValue("datePublished",out datePublished);
      Assert.Equal("2023",datePublished.ToString());
    }
}
