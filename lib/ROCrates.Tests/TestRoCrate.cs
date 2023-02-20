namespace ROCrates.Tests;

public class TestRoCrate
{
  [Fact]
  public void TestResolveId()
  {
    string validUrl = "https://doi.org/10.4225/59/59672c09f4a4b";
    string invalidUrl = "htps/doi.org/10.4225/59/59672c09f4a4b";
    
    var crate = new ROCrate("my-test.zip");
    string resultValidUrl = crate.ResolveId(validUrl);
    Assert.Equal(validUrl,resultValidUrl);
    
    string resultInvalidUrl = crate.ResolveId(invalidUrl);
    Assert.StartsWith("arcp://", resultInvalidUrl);
    Assert.EndsWith(invalidUrl, resultInvalidUrl);

  }
}
