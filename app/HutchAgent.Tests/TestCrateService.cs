using System.IO.Compression;
using System.Text.Json.Nodes;
using HutchAgent.Config;
using HutchAgent.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json.Linq;
using ROCrates;
using ROCrates.Models;
using File = System.IO.File;

namespace HutchAgent.Tests;

public class TestCrateMergeService
{
  [Fact]
  public void MergeCrates_Extract_ZipToDestinationDataOutputs()
  {
    // Arrange
    var publisher = Options.Create(new PublisherOptions(){ Name = "TRE name" });
    var logger = new Mock<ILogger<CrateService>>();
    var pathToOutputDir = Path.Combine("data", "outputs");
    var destinationDir = new DirectoryInfo("save/here/");
    var zipFile = new FileInfo("test-zip.zip");
    var metaFile = new FileInfo("ro-crate-metadata.json");
    var expectedFile = new FileInfo(Path.Combine(destinationDir.FullName, pathToOutputDir, metaFile.Name));
    
    Directory.CreateDirectory(destinationDir.ToString());
    File.Create(metaFile.Name).Close();

    using (var zipArchive = ZipFile.Open(zipFile.ToString(), ZipArchiveMode.Create))
    {
        zipArchive.CreateEntryFromFile(metaFile.FullName, metaFile.Name);
    }
    
    var service = new CrateService(publisher,logger.Object);

    // Act
    service.MergeCrates(zipFile.Name, destinationDir.ToString());

    // Assert
    Assert.True(expectedFile.Exists);

    // Clean up
    if (zipFile.Exists) zipFile.Delete();
    if (metaFile.Exists) metaFile.Delete();
    if (destinationDir.Exists) destinationDir.Parent!.Delete(true);
  }

  [Fact]
  public void ZipCrate_Zips_DestinationToParent()
  {
    // Arrange
    var publisher = Options.Create(new PublisherOptions(){ Name = "TRE name" });
    var logger = new Mock<ILogger<CrateService>>();
    var destinationDir = new DirectoryInfo("save2/here/");
    var zipFile = new FileInfo(Path.Combine(destinationDir.ToString(), "test-zip.zip"));
    var expectedFile = new FileInfo($"save2/{destinationDir.Name}-merged.zip");

    Directory.CreateDirectory(destinationDir.ToString());
    File.Create(zipFile.ToString()).Close();

    var service = new CrateService(publisher,logger.Object);

    // Act
    service.ZipCrate(destinationDir.ToString());

    // Assert
    Assert.True(expectedFile.Exists);

    // Clean up
    if (zipFile.Exists) zipFile.Delete();
    if (destinationDir.Exists) destinationDir.Parent!.Delete(true);
  }

  [Fact]
  public void MergeCrates_Throws_WhenDestinationNonExistent()
  {
    // Arrange
    var publisher = Options.Create(new PublisherOptions(){ Name = "TRE name" });
    var logger = new Mock<ILogger<CrateService>>();
    var zipFileName = "my-file.zip";
    var destinationDir = "non/existent/dir/";
    Directory.CreateDirectory("non/existent/");
    var service = new CrateService(publisher,logger.Object);

    // Act
    var action = () => service.MergeCrates(zipFileName, destinationDir);

    // Assert
    Assert.Throws<DirectoryNotFoundException>(action);

    // Clean up
    if (Directory.Exists("non/")) Directory.Delete("non/", true);
  }

  [Fact]
  public void ZipCrate_Throws_WhenDestinationNonExistent()
  {
    // Arrange
    var publisher = Options.Create(new PublisherOptions(){ Name = "TRE name" });
    var logger = new Mock<ILogger<CrateService>>();
    var destinationDir = "non/existent/dir/";
    var service = new CrateService(publisher,logger.Object);

    // Act
    var action = () => service.ZipCrate(destinationDir);

    // Assert
    Assert.Throws<DirectoryNotFoundException>(action);
  }

  [Fact]
  public void UpdateMetadata_Throws_WhenSourceNonExistent()
  {
    // Arrange
    var publisher = Options.Create(new PublisherOptions(){ Name = "TRE name" });
    var logger = new Mock<ILogger<CrateService>>();
    var pathToMetadata = "non/existent/ro-crate-metadata.json";
    var service = new CrateService(publisher,logger.Object);

    // Act
    var action = () => service.UpdateMetadata(pathToMetadata);

    // Assert
    Assert.Throws<FileNotFoundException>(action);
  }

  [Fact]
  public void UpdateMetadata_Adds_MergedEntity()
  {
    // Arrange
    var publisher = Options.Create(new PublisherOptions(){ Name = "TRE name" });
    var logger = new Mock<ILogger<CrateService>>();
    var pathToOutputDir = Path.Combine("data", "outputs");
    var crate = new ROCrate();
    var rootDataset = new RootDataset(crate: crate);
    Directory.CreateDirectory("some-source");
    var dataset = new Dataset(crate: crate, source: "some-source");
    crate.Add(rootDataset, dataset);
    crate.Metadata.Write("./");
    
    if (!File.Exists(crate.Metadata.Id)) 
      throw new FileNotFoundException("Could not locate the metadata file.");
    var metaFile = new FileInfo(crate.Metadata.Id);

    if (!Directory.Exists(metaFile.DirectoryName)) 
      throw new FileNotFoundException("Could not locate the metadata directory.");
    var outputDirToAdd = Path.Combine(metaFile.DirectoryName, pathToOutputDir);
    Directory.CreateDirectory(outputDirToAdd);
    
    var pattern1 = "\"@id\": " 
                   + "\""
                   + $"{Path.GetRelativePath(metaFile.DirectoryName, Path.Combine(metaFile.DirectoryName, pathToOutputDir))}/" 
                   + "\"";
    var pattern2 = "\"publisher\": ";
    var pattern3 = "\"@id\": " 
                   + "\""
                   + $"{publisher.Value.Name}" 
                   + "\"";
    var pattern4 = "\"datePublished\": ";
    
    var service = new CrateService(publisher,logger.Object);

    // Act
    service.UpdateMetadata(metaFile.DirectoryName);
    var output = File.ReadAllText(crate.Metadata.Id);

    
    // Assert
    Assert.Contains(pattern1, output);
    Assert.Contains(pattern2, output);
    Assert.Contains(pattern3, output);
    Assert.Contains(pattern4, output);
    
    // Clean up
    if (File.Exists(crate.Metadata.Id)) File.Delete(crate.Metadata.Id);
    if (Directory.Exists(outputDirToAdd)) Directory.Delete(outputDirToAdd, recursive: true);
  }

  [Fact]
  public void CreateDisclosureCheck_CreatesEntity()
  {
    // Arrange
    var publisher = Options.Create(new PublisherOptions() { Name = "TRE name" });
    var logger = new Mock<ILogger<CrateService>>();
    
  }
}
