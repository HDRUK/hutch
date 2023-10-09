using System.Globalization;
using System.IO.Compression;
using System.Text.Json;
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

public class TestFiveSafesCrateService : IClassFixture<CrateServiceFixture>
{
  private const string _publisherId = "https://tre78.example.com";
  private const string _licenseUrl = "https://spdx.org/licenses/CC-BY-4.0";
  private const string _licenseId = "CC-BY-4.0";
  private const string _licenseName = "Creative Commons Attribution 4.0 International";

  private readonly CrateServiceFixture _crateServiceFixture;
  private readonly IOptions<PathOptions> _paths;
  private readonly Mock<ILogger<FiveSafesCrateService>> _logger;

  public TestFiveSafesCrateService(CrateServiceFixture crateServiceFixture)
  {
    _crateServiceFixture = crateServiceFixture;

    _paths = Options.Create<PathOptions>(new());

    _logger = new Mock<ILogger<FiveSafesCrateService>>();
  }

  // TODO FinalizeMetadata (replacing UpdateMetadata) isn't pure enough to test.
  // TODO Refactoring needed into pure testable components
  // [Fact]
  // public void UpdateMetadata_Adds_MergedEntity()
  // {
  //   // Arrange
  //   var pathToOutputDir = "outputs";
  //   var crate = new ROCrate();
  //   Directory.CreateDirectory(_crateServiceFixture.ResultsCrateDirName);
  //   var dataset = new Dataset(crate: crate, source: _crateServiceFixture.ResultsCrateDirName);
  //   var createAction = new Entity();
  //   createAction.SetProperty("@type", "CreateAction");
  //   crate.Add(dataset, createAction);
  //   crate.Metadata.Write(_crateServiceFixture.InputCrateDirName);
  //
  //   var outputDirToAdd = Path.Combine(_crateServiceFixture.InputCrateDirName, pathToOutputDir);
  //   Directory.CreateDirectory(outputDirToAdd);
  //
  //   var relativePathToOutputs = Path.GetRelativePath(
  //     _crateServiceFixture.InputCrateDirName,
  //     Path.Combine(_crateServiceFixture.InputCrateDirName, pathToOutputDir));
  //   var pattern1 = "\"@id\": "
  //                  + "\""
  //                  + $"{relativePathToOutputs}/"
  //                  + "\"";
  //   var pattern2 = "\"publisher\": ";
  //   var pattern3 = "\"@id\": "
  //                  + "\""
  //                  + $"{_publishOptions.Value.Publisher!.Id}"
  //                  + "\"";
  //   var pattern4 = "\"datePublished\": ";
  //
  //   var service = new FiveSafesCrateService(_paths, _publishOptions, _logger.Object);
  //   var startTime = DateTime.Now;
  //   var endTime = startTime + TimeSpan.FromMinutes(2);
  //   var job = new Models.WorkflowJob { ExecutionStartTime = startTime, EndTime = endTime };
  //   // Act
  //   service.UpdateMetadata(_crateServiceFixture.InputCrateDirName, job);
  //   var output = File.ReadAllText(Path.Combine(_crateServiceFixture.InputCrateDirName, crate.Metadata.Id));
  //
  //
  //   // Assert
  //   Assert.Contains(pattern1, output);
  //   Assert.Contains(pattern2, output);
  //   Assert.Contains(pattern3, output);
  //   Assert.Contains(pattern4, output);
  //   Assert.Contains(startTime.ToString(CultureInfo.InvariantCulture), output);
  //   Assert.Contains(endTime.ToString(CultureInfo.InvariantCulture), output);
  //
  //   // Clean up
  //   if (File.Exists(crate.Metadata.Id)) File.Delete(crate.Metadata.Id);
  //   if (Directory.Exists(outputDirToAdd)) Directory.Delete(outputDirToAdd, recursive: true);
  // }

  [Fact]
  public void AddLicense_AddsLicenseToMetadata()
  {
    // Arrange
    var crate = new ROCrate();

    var service = new FiveSafesCrateService(
      _paths,
      Options.Create<CratePublishingOptions>(new()
      {
        License = new()
        {
          Uri = _licenseUrl,
          Properties = new Dictionary<string, JsonNode?>
          {
            ["name"] = _licenseName,
            ["identifier"] = _licenseId
          }
        }
      }),
      _logger.Object);

    // Act
    service.AddLicense(crate);

    // Assert
    var license = crate.Entities[_licenseUrl];
    
    Assert.NotNull(license); // We found it? We must have got the entity id right (the license URL)
    
    Assert.Equal(_licenseName, license.Properties["name"]?.ToString());
    Assert.Equal(_licenseId, license.Properties["identifier"]?.ToString());
  }
}

public class CrateServiceFixture : IDisposable
{
  public string InputCrateDirName { get; } = Guid.NewGuid().ToString();
  public string ResultsCrateDirName { get; } = Guid.NewGuid().ToString();

  public CrateServiceFixture()
  {
    Directory.CreateDirectory(InputCrateDirName.BagItPayloadPath());
    Directory.CreateDirectory(ResultsCrateDirName);
  }

  public void Dispose()
  {
    if (Directory.Exists(InputCrateDirName)) Directory.Delete(InputCrateDirName, recursive: true);
    if (Directory.Exists(ResultsCrateDirName)) Directory.Delete(ResultsCrateDirName, recursive: true);
  }
}
