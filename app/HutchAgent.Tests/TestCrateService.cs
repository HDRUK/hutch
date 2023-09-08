using System.Globalization;
using System.IO.Compression;
using System.Text.Json.Nodes;
using HutchAgent.Config;
using HutchAgent.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using ROCrates;
using ROCrates.Models;
using File = System.IO.File;

namespace HutchAgent.Tests;

public class TestCrateService : IClassFixture<CrateServiceFixture>
{
  private readonly CrateServiceFixture _crateServiceFixture;
  private readonly IOptions<PathOptions> _paths;
  private readonly IOptions<LicenseOptions> _license;
  private readonly IOptions<PublisherOptions> _publisher;
  private readonly Mock<ILogger<CrateService>> _logger;

  public TestCrateService(CrateServiceFixture crateServiceFixture)
  {
    _crateServiceFixture = crateServiceFixture;

    _paths = Options.Create<PathOptions>(new()
    {
      // TODO
    });

    var licenseProps = new JsonObject();
    licenseProps.Add("name", "Creative Commons Attribution 4.0 International");
    licenseProps.Add("identifier", "CC-BY-4.0");
    _license = Options.Create<LicenseOptions>(new()
    {
      Uri = "https://spdx.org/licenses/CC-BY-4.0",
      Properties = licenseProps
    });

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
    Directory.CreateDirectory(_crateServiceFixture.ResultsCrateDirName);
    var dataset = new Dataset(crate: crate, source: _crateServiceFixture.ResultsCrateDirName);
    var createAction = new Entity();
    createAction.SetProperty("@type", "CreateAction");
    crate.Add(dataset, createAction);
    crate.Metadata.Write(_crateServiceFixture.InputCrateDirName);

    var outputDirToAdd = Path.Combine(_crateServiceFixture.InputCrateDirName, pathToOutputDir);
    Directory.CreateDirectory(outputDirToAdd);

    var relativePathToOutputs = Path.GetRelativePath(
      _crateServiceFixture.InputCrateDirName,
      Path.Combine(_crateServiceFixture.InputCrateDirName, pathToOutputDir));
    var pattern1 = "\"@id\": "
                   + "\""
                   + $"{relativePathToOutputs}/"
                   + "\"";
    var pattern2 = "\"publisher\": ";
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
    service.UpdateMetadata(_crateServiceFixture.InputCrateDirName, job);
    var output = File.ReadAllText(Path.Combine(_crateServiceFixture.InputCrateDirName, crate.Metadata.Id));


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

public class CrateServiceFixture : IDisposable
{
  public string InputCrateDirName { get; } = Guid.NewGuid().ToString();
  public string ResultsCrateDirName { get; } = Guid.NewGuid().ToString();

  public CrateServiceFixture()
  {
    Directory.CreateDirectory(InputCrateDirName);
    Directory.CreateDirectory(ResultsCrateDirName);
  }

  public void Dispose()
  {
    if (Directory.Exists(InputCrateDirName)) Directory.Delete(InputCrateDirName, recursive: true);
    if (Directory.Exists(ResultsCrateDirName)) Directory.Delete(ResultsCrateDirName, recursive: true);
  }
}
