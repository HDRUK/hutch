using System.Globalization;
using System.IO.Compression;
using HutchAgent.Config;
using HutchAgent.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using ROCrates;
using ROCrates.Models;
using File = System.IO.File;

namespace HutchAgent.Tests;

public class TestCrateService
{
  private readonly IOptions<PathOptions> _paths;
  private readonly IOptions<LicenseOptions> _license;
  private readonly IOptions<PublisherOptions> _publisher;
  private readonly Mock<ILogger<CrateService>> _logger;

  public TestCrateService()
  {
    _paths = Options.Create<PathOptions>(new()
    {
      // TODO
    });
    _license = Options.Create<LicenseOptions>(new());
    _publisher = Options.Create(new PublisherOptions { Name = "TRE name" });
    _logger = new Mock<ILogger<CrateService>>();
  }

  [Fact]
  public void MergeCrates_Extract_ZipToDestinationDataOutputs()
  {
    // Arrange
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

    var service = new CrateService(_paths, _publisher, _logger.Object, _license);

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

    var service = new CrateService(_paths, _publisher, _logger.Object, _license);

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
    var service = new CrateService(_paths, _publisher, _logger.Object, _license);

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
    var service = new CrateService(_paths, _publisher, _logger.Object, _license);

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
    var startTime = DateTime.Now;
    var endTime = startTime + TimeSpan.FromMinutes(2);
    var job = new Models.WorkflowJob { ExecutionStartTime = startTime, EndTime = endTime };
    var service = new CrateService(_paths, _publisher, _logger.Object, _license);

    // Act
    var action = () => service.UpdateMetadata(pathToMetadata, job);

    // Assert
    Assert.Throws<FileNotFoundException>(action);
  }

  [Fact]
  public void UpdateMetadata_Adds_MergedEntity()
  {
    // Arrange
    var pathToOutputDir = "outputs";
    var crate = new ROCrate();
    Directory.CreateDirectory("some-source");
    var dataset = new Dataset(crate: crate, source: "some-source");
    var createAction = new Entity();
    createAction.SetProperty("@type", "CreateAction");
    crate.Add(dataset, createAction);
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
    var pattern2 = "\"_publisher\": ";
    var pattern3 = "\"@id\": "
                   + "\""
                   + $"{_publisher.Value.Name}"
                   + "\"";
    var pattern4 = "\"datePublished\": ";

    var service = new CrateService(_paths, _publisher, _logger.Object, _license);
    var startTime = DateTime.Now;
    var endTime = startTime + TimeSpan.FromMinutes(2);
    var job = new Models.WorkflowJob { ExecutionStartTime = startTime, EndTime = endTime };
    // Act
    service.UpdateMetadata(metaFile.DirectoryName, job);
    var output = File.ReadAllText(crate.Metadata.Id);


    // Assert
    Assert.Contains(pattern1, output);
    Assert.Contains(pattern2, output);
    Assert.Contains(pattern3, output);
    Assert.Contains(pattern4, output);
    Assert.Contains(startTime.ToString(CultureInfo.InvariantCulture), output);
    Assert.Contains(endTime.ToString(CultureInfo.InvariantCulture), output);

    // Clean up
    if (File.Exists(crate.Metadata.Id)) File.Delete(crate.Metadata.Id);
    if (Directory.Exists(outputDirToAdd)) Directory.Delete(outputDirToAdd, recursive: true);
  }
}
