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
  public void ResolveId_Combines_GoodAndBad_Uris()
  {
    string validUrl = "https://doi.org/10.4225/59/59672c09f4a4b";
    string invalidUrl = "https://do i.org/10.4225/59/59672c09f4a4b";
    var crate = new ROCrate("my-test.zip");

    string resultValidUrl = crate.ResolveId(validUrl);
    Assert.Equal(validUrl, resultValidUrl);

    string resultInvalidUrl = crate.ResolveId(invalidUrl);
    Assert.StartsWith("arcp://", resultInvalidUrl);
    Assert.EndsWith(invalidUrl, resultInvalidUrl);
  }

  [Fact]
  public void Add_Adds_RootDataset()
  {
    var roCrate = new ROCrate();
    var rootDataset = new RootDataset(roCrate);

    roCrate.Add(rootDataset);
    Assert.Equal(roCrate.RootDataset.Id, rootDataset.Id);
    Assert.Equal(roCrate.RootDataset.Properties, rootDataset.Properties);

    Assert.True(roCrate.Entities.ContainsKey(rootDataset.GetCanonicalId()));
    Assert.True(roCrate.Entities.TryGetValue(rootDataset.GetCanonicalId(), out var recoveredRootDataset));
    Assert.IsType<RootDataset>(recoveredRootDataset);
  }

  [Fact]
  public void Add_Adds_Metadata()
  {
    var roCrate = new ROCrate();
    var metadata = new Metadata(roCrate);

    roCrate.Add(metadata);
    Assert.Equal(roCrate.Metadata.Id, metadata.Id);
    Assert.Equal(roCrate.Metadata.Properties, metadata.Properties);

    Assert.True(roCrate.Entities.ContainsKey(metadata.GetCanonicalId()));
    Assert.True(roCrate.Entities.TryGetValue(metadata.GetCanonicalId(), out var recoveredMetadata));
    Assert.IsType<Metadata>(recoveredMetadata);
  }

  [Fact]
  public void Add_Adds_ObjetsOfDifferentTypes()
  {
    var roCrate = new ROCrate();
    var person = new Person(roCrate);
    var file = new File(roCrate, source: "my-test-file.txt");
    var dataset = new Dataset(roCrate, source: "my-data-dir/");

    roCrate.Add(person, file, dataset);

    Assert.True(roCrate.Entities.ContainsKey(person.GetCanonicalId()));
    Assert.True(roCrate.Entities.TryGetValue(person.GetCanonicalId(), out var recoveredPerson));
    Assert.IsType<Person>(recoveredPerson);

    Assert.True(roCrate.Entities.ContainsKey(file.GetCanonicalId()));
    Assert.True(roCrate.Entities.TryGetValue(file.GetCanonicalId(), out var recoveredFile));
    Assert.IsType<File>(recoveredFile);

    Assert.True(roCrate.Entities.ContainsKey(dataset.GetCanonicalId()));
    Assert.True(roCrate.Entities.TryGetValue(dataset.GetCanonicalId(), out var recoveredDataset));
    Assert.IsType<Dataset>(recoveredDataset);
  }

  [Fact]
  public void Save_Creates_DirectoryWithFiles()
  {
    // Arrange
    var file1 = new FileInfo(Guid.NewGuid().ToString());
    file1.Create().Close();
    var file2 = new FileInfo(Guid.NewGuid().ToString());
    file2.Create().Close();
    var outputDir = Guid.NewGuid().ToString();

    var roCrate = new ROCrate();
    var fileObject1 = new File(crate: roCrate, source: file1.ToString());
    var fileObject2 = new File(crate: roCrate, source: file2.ToString());
    roCrate.Add(fileObject1, fileObject2);

    // Act
    roCrate.Save(outputDir);

    // Assert
    Assert.True(System.IO.File.Exists(Path.Combine(outputDir, file1.Name)));
    Assert.True(System.IO.File.Exists(Path.Combine(outputDir, file2.Name)));

    // Clean up
    if (file1.Exists) file1.Delete();
    if (file2.Exists) file2.Delete();
    if (Directory.Exists(outputDir)) Directory.Delete(outputDir, recursive: true);
  }

  [Fact]
  public void Save_Creates_ZipWithFiles()
  {
    // Arrange
    var file1 = new FileInfo(Guid.NewGuid().ToString());
    file1.Create().Close();
    var file2 = new FileInfo(Guid.NewGuid().ToString());
    file2.Create().Close();
    var outputDir = Guid.NewGuid().ToString();
    var outputZipFile = $"{outputDir}.zip";

    var roCrate = new ROCrate();
    var fileObject1 = new File(crate: roCrate, source: file1.ToString());
    var fileObject2 = new File(crate: roCrate, source: file2.ToString());
    roCrate.Add(fileObject1, fileObject2);

    // Act
    roCrate.Save(outputDir, zip: true);

    // Assert
    Assert.True(System.IO.File.Exists(outputZipFile));

    // Clean up
    if (file1.Exists) file1.Delete();
    if (file2.Exists) file2.Delete();
    if (Directory.Exists(outputDir)) Directory.Delete(outputDir, recursive: true);
    if (System.IO.File.Exists(outputZipFile)) System.IO.File.Delete(outputZipFile);
  }
}
