using System.IO.Compression;
using System.Text.Json.Nodes;

namespace HutchAgent.Services;

/// <summary>
/// This service merges an output RO-Crate back into its input RO-Crate.
/// </summary>
public class CrateMergerService
{
  /// <summary>
  /// Copy a source zipped RO-Crate into an unzipped destination RO-Crate and zip the
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

    // Copy the result RO-Crate (`file`) into the unzipped original RO-Crate
    File.Copy(sourceZip, Path.Combine(mergeInto, Path.GetFileName(sourceZip)));
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
  /// Update the metadata of a 
  /// </summary>
  /// <param name="pathToMetadata"></param>
  /// <param name="fileToAdd"></param>
  /// <exception cref="FileNotFoundException"></exception>
  /// <exception cref="InvalidDataException"></exception>
  public void UpdateMetadata(string pathToMetadata, string fileToAdd)
  {
    if (!File.Exists(pathToMetadata))
      throw new FileNotFoundException("Could not locate the metadata for the RO-Crate.");
    var metadataJson = File.ReadAllText(pathToMetadata);
    var metadata = JsonNode.Parse(metadataJson);
    if (metadata is null) throw new InvalidDataException($"{pathToMetadata} is not a valid JSON file.");

    if (!metadata.AsObject().TryGetPropertyValue("@graph", out var graph))
      throw new InvalidDataException("Cannot find entities in the RO-Crate metadata.");

    var rootDatasetProperties = graph.AsArray().First(g => g["@id"].ToString() == "./");
    var rootDatasetPath = rootDatasetProperties.GetPath();
    var file = new ROCrates.Models.File(source: fileToAdd).Serialize();
    rootDatasetProperties["hasPart"].AsArray().Add(JsonNode.Parse(file));

    File.WriteAllText(pathToMetadata, metadata.ToString());
  }
}
