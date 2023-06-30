using System.IO.Compression;
using HutchAgent.Services;
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
    var destinationDir = new DirectoryInfo("save/here/");
    var zipFile = new FileInfo("test-zip.zip");
    var metaFile = new FileInfo("ro-crate-metadata.json");
    var expectedFile = new FileInfo(Path.Combine(destinationDir.ToString(), "Data", "outputs", metaFile.Name));
    
    Directory.CreateDirectory(destinationDir.ToString());
    File.Create(metaFile.Name).Close();

    using (var zipArchive = ZipFile.Open(zipFile.ToString(), ZipArchiveMode.Create))
    {
        zipArchive.CreateEntryFromFile(metaFile.FullName, metaFile.Name);
    }
    
    var service = new CrateMergerService();

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
    var destinationDir = new DirectoryInfo("save2/here/");
    var zipFile = new FileInfo(Path.Combine(destinationDir.ToString(), "test-zip.zip"));
    var expectedFile = new FileInfo($"save2/{destinationDir.Name}-merged.zip");

    Directory.CreateDirectory(destinationDir.ToString());
    File.Create(zipFile.ToString()).Close();

    var service = new CrateMergerService();

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
    var zipFileName = "my-file.zip";
    var destinationDir = "non/existent/dir/";
    Directory.CreateDirectory("non/existent/");
    var service = new CrateMergerService();

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
    var destinationDir = "non/existent/dir/";
    var service = new CrateMergerService();

    // Act
    var action = () => service.ZipCrate(destinationDir);

    // Assert
    Assert.Throws<DirectoryNotFoundException>(action);
  }

  [Fact]
  public void UpdateMetadata_Throws_WhenSourceNonExistent()
  {
    // Arrange
    var pathToMetadata = "non/existent/ro-crate-metadata.json";
    var filToAdd = "my-file.txt";
    var service = new CrateMergerService();

    // Act
    var action = () => service.UpdateMetadata(pathToMetadata, filToAdd);

    // Assert
    Assert.Throws<FileNotFoundException>(action);
  }

  [Fact]
  public void UpdateMetadata_Adds_MergedEntity()
  {
    // Arrange
    var crate = new ROCrate();
    var rootDataset = new RootDataset(crate: crate);
    var dataset = new Dataset(crate: crate, source: "some-source");
    crate.Add(rootDataset, dataset);
    crate.Metadata.Write("./");


    var fileToAdd = new FileInfo("fileToAdd.zip");
    var fileObject = new ROCrates.Models.File(crate: crate, source: fileToAdd.Name);
    fileToAdd.Create().Close();
    crate.Add(fileObject);

    var service = new CrateMergerService();

    // Act
    service.UpdateMetadata(crate.Metadata.Id, fileToAdd.Name);
    var output = File.ReadAllText(crate.Metadata.Id);
    var pattern = "\"@id\": " + "\"" + $"{fileToAdd.Name}" + "\"";

    // Assert
    Assert.Contains(pattern, output);

    // Clean up
    if (File.Exists(crate.Metadata.Id)) File.Delete(crate.Metadata.Id);
    if (fileToAdd.Exists) fileToAdd.Delete();
  }
}
