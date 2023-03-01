using ROCrates.Models;
using Xunit.Abstractions;
using File = ROCrates.Models.File;

namespace ROCrates.Tests;

public class TestRoCrate
{
  private readonly ITestOutputHelper _testOutputHelper;

  public TestRoCrate(ITestOutputHelper testOutputHelper)
  {
    _testOutputHelper = testOutputHelper;
  }

  [Fact]
  public void TestResolveId_CombinesGoodAndBad_Uris()
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

  [Fact]
  public void TestResolveId_TrimsTrailingSlash()
  {
    string noSlash = "https://doi.org/10.4225/59/59672c09f4a4b";
    string withSlash = noSlash + '/';
    var crate = new ROCrate("my-test.zip");
    
    string resultNoSlash = crate.ResolveId(noSlash);
    Assert.Equal(noSlash,resultNoSlash);
    
    string resultWithSlash = crate.ResolveId(withSlash);
    Assert.EndsWith(withSlash.TrimEnd('/'), resultWithSlash);
  }

  
  [Fact]
  public void TestAdd_AddsRootDataset()
  {
    var roCrate = new ROCrate();
    var rootDataset = new RootDataset(roCrate);

    roCrate.Add(rootDataset);
    Assert.Equal(roCrate.RootDataset.Identifier, rootDataset.Identifier);
    Assert.Equal(roCrate.RootDataset.Properties, rootDataset.Properties);

    Assert.True(roCrate.Entities.ContainsKey(rootDataset.GetCanonicalId()));
    Assert.True(roCrate.Entities.TryGetValue(rootDataset.GetCanonicalId(), out var recoveredRootDataset));
    Assert.IsType<RootDataset>(recoveredRootDataset);
  }
  
  [Fact]
  public void TestAdd_AddsMetadata()
  {
    var roCrate = new ROCrate();
    var metadata = new Metadata(roCrate);

    roCrate.Add(metadata);
    Assert.Equal(roCrate.Metadata.Identifier, metadata.Identifier);
    Assert.Equal(roCrate.Metadata.Properties, metadata.Properties);

    Assert.True(roCrate.Entities.ContainsKey(metadata.GetCanonicalId()));
    Assert.True(roCrate.Entities.TryGetValue(metadata.GetCanonicalId(), out var recoveredMetadata));
    Assert.IsType<Metadata>(recoveredMetadata);
  }
  
  [Fact]
  public void TestAdd_AddsObjetsOfDifferentTypes()
  {
    var roCrate = new ROCrate();
    var person = new Person(roCrate);
    var rootDataset = new RootDataset(roCrate);
    var metadata = new Metadata(roCrate);
    var file = new File(roCrate, source: "my-test-file.txt");

    roCrate.Add(person, rootDataset, file, metadata);
    Assert.Equal(roCrate.RootDataset.Identifier, rootDataset.Identifier);
    Assert.Equal(roCrate.RootDataset.Properties, rootDataset.Properties);
    Assert.IsType<RootDataset>(roCrate.RootDataset);
    Assert.Equal(roCrate.Metadata.Identifier, metadata.Identifier);
    Assert.IsType<Metadata>(roCrate.Metadata);
  }
}
