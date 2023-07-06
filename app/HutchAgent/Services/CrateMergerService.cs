using System.IO.Compression;
using System.Text.Json.Nodes;

namespace HutchAgent.Services;

/// <summary>
/// This service merges an output RO-Crate back into its input RO-Crate.
/// </summary>
public class CrateMergerService
{
  private readonly string _pathToOutputDir = Path.Combine("Data","outputs");
  /// <summary>
  /// Extract a source zipped RO-Crate into an unzipped destination RO-Crate `Data/outputs` directory and zip the
  /// destination RO-Crate.
  /// </summary>
  /// <param name="sourceZip"></param>
  /// <param name="mergeInto"></param>
  /// <exception cref="DirectoryNotFoundException">
  /// The directory you are attempting to merge doesn't exists.
  /// The parent of the destination directory couldn't be found or does not exists.
  /// </exception>
  public void MergeCrates(string sourceZip, string mergeInto)
  {
    // Get information on destination and make sure it exists
    var destinationInfo = new DirectoryInfo(mergeInto);
    if (!destinationInfo.Exists) throw new DirectoryNotFoundException($"{mergeInto} does not exist.");

    // Create output directory `Data/outputs/` to extract execution crate
    var outputDir = Path.Combine(mergeInto, _pathToOutputDir);
    Directory.CreateDirectory(outputDir);
    
    // Extract the result (RO-Crate) into the unzipped original RO-Crate
    ZipFile.ExtractToDirectory(sourceZip, outputDir);
  }

  /// <summary>
  /// Zip a directory containing the contents of an RO-Crate, saving it in the directory's parent directory
  /// with a name like "cratePath-merged.zip".
  /// </summary>
  /// <param name="cratePath">The path to the unzipped RO-Crate.</param>
  /// <exception cref="DirectoryNotFoundException">
  /// Thrown when the directory of the RO-Crate does not exists.
  /// </exception>
  public void ZipCrate(string cratePath)
  {
    var dirInfo = new DirectoryInfo(cratePath);
    if (!dirInfo.Exists) throw new DirectoryNotFoundException($"{cratePath} does not exists.");
    var parent = dirInfo.Parent;
    var zipFile = $"{dirInfo.Name}-merged.zip";
    ZipFile.CreateFromDirectory(cratePath, Path.Combine(parent!.ToString(), zipFile));
  }

  /// <summary>
  /// Update the target metadata file in an RO-Crate.
  /// </summary>
  /// <param name="pathToMetadata">The path to the metadata file that needs updating.</param>
  /// <exception cref="FileNotFoundException">
  /// Metadata file could not be found.
  /// </exception>
  /// <exception cref="InvalidDataException">The metadata file is invalid.</exception>
  public void UpdateMetadata(string pathToMetadata)
  {
    if (!File.Exists(pathToMetadata))
      throw new FileNotFoundException("Could not locate the metadata for the RO-Crate.");

    var rootDirInfo = new DirectoryInfo(pathToMetadata).Parent; // get root dir info
    
    var folderToAdd = Path.Combine(rootDirInfo.ToString(), _pathToOutputDir);
    
    if (!Directory.Exists(folderToAdd))
      throw new FileNotFoundException("Could not locate the folder to add to the metadata.");
    
    var metadataJson = File.ReadAllText(pathToMetadata);
    var metadata = JsonNode.Parse(metadataJson);
    if (metadata is null) throw new InvalidDataException($"{pathToMetadata} is not a valid JSON file.");

    if (!metadata.AsObject().TryGetPropertyValue("@graph", out var graph))
      throw new InvalidDataException("Cannot find entities in the RO-Crate metadata.");

    var rootDatasetProperties = graph.AsArray().First(g => g["@id"].ToString() == "./");
    var folder = new ROCrates.Models.Dataset(source: Path.GetRelativePath(rootDirInfo.ToString(), folderToAdd)).Serialize();
    rootDatasetProperties["hasPart"].AsArray().Add(JsonNode.Parse(folder));

    File.WriteAllText(pathToMetadata, metadata.ToString());
  }
  
  /// <summary>
  /// Delete container images
  /// </summary>
  /// <param name="pathToImagesDir">The path to container images directory.</param>
  public void DeleteContainerImages(string pathToImagesDir)
  {
    var images = Directory.GetFiles(pathToImagesDir, "*.img");
    foreach (var image in images) 
      File.Delete(image); // TODO
  }
}
