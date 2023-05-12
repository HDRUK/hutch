using HutchAgent.Services;

namespace HutchAgent.Tests;

public class TestCrateMergeService
{
  [Fact]
  public void MergeCrates_Copies_ZipToDestination()
  {
    // Arrange
    var destinationDir = new DirectoryInfo("save/here/");
    var zipFile = new FileInfo("test-zip.zip");
    File.Create(zipFile.Name).Close();
    Directory.CreateDirectory(destinationDir.ToString());

    var service = new CrateMergerService();

    // Act
    service.MergeCrates(zipFile.Name, destinationDir.ToString());

    // Assert
    Assert.True(File.Exists(Path.Combine(destinationDir.ToString(), zipFile.Name)));

    // Clean up
    if (zipFile.Exists) zipFile.Delete();
    if (destinationDir.Exists) destinationDir.Parent!.Delete(true);
  }

  [Fact]
  public void MergeCrates_Zips_DestinationToParent()
  {
    // Arrange

    // Act

    // Assert
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
}
