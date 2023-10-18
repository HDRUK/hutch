using System.IO.Abstractions.TestingHelpers;
using System.Text.RegularExpressions;
using HutchAgent.Config;
using HutchAgent.Services;
using HutchAgent.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Serilog.Core;

namespace HutchAgent.Tests.WorkflowTriggerServiceTests;

public class TestSelectivelyDelete
{
  private readonly Mock<ILogger<MinioStoreService>> _logger = new();

  [Fact]
  public async void SelectivelyDelete_DeletesNonMatchingFiles()
  {
    // Arrange
    // TODO dummy fs
    var fs = new MockFileSystem(new Dictionary<string, MockFileData>()
    {
      { "/var/somefile.txt", new MockFileData("") },
      { "/var/somefile-meta.json", new MockFileData("") },
      { "/var/someotherfile.img", new MockFileData("") }
    });

    var sut = new FileSystemUtility(fs);

    // Act
    sut.SelectivelyDelete("/var", new Regex(@".*meta\.json$"));

    // Assert
    var files = fs.Directory.EnumerateFiles("var").ToList();
    Assert.Single(files);
    Assert.Equal("/var/somefile-meta.json", files.Single());
  }
}
