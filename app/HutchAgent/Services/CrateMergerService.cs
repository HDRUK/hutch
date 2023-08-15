using System.IO.Compression;
using HutchAgent.Config;
using Microsoft.Extensions.Options;
using ROCrates;
using ROCrates.Models;
using File = System.IO.File;

namespace HutchAgent.Services;

/// <summary>
/// This service merges an output RO-Crate back into its input RO-Crate.
/// </summary>
public class CrateMergerService
{
  private readonly string _pathToOutputDir = Path.Combine("data", "outputs");
  private readonly PublisherOptions _publisherOptions;

  public CrateMergerService(IOptions<PublisherOptions> publisher)
  {
    _publisherOptions = publisher.Value;
  }

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
    if (!File.Exists(Path.Combine(pathToMetadata, "ro-crate-metadata.json")))
      throw new FileNotFoundException("Could not locate the metadata for the RO-Crate.");

    var metaDirInfo = new DirectoryInfo(pathToMetadata);

    var outputsDirToAdd = Path.Combine(metaDirInfo.FullName, _pathToOutputDir);
    if (!Directory.Exists(outputsDirToAdd))
      throw new DirectoryNotFoundException("Could not locate the folder to add to the metadata.");

    // Create entity to represent the outputs folder
    var outputs = new Dataset(source: Path.GetRelativePath(metaDirInfo.FullName, outputsDirToAdd));
    // Create entities representing the files in the outputs folder
    var outputFiles = Directory.EnumerateFiles(outputsDirToAdd, "*", SearchOption.AllDirectories).Select(file =>
      new ROCrates.Models.File(source: Path.GetRelativePath(metaDirInfo.FullName, file))).ToList();

    var crate = new ROCrate();
    crate.Initialise(metaDirInfo.FullName);
    crate.RootDataset.SetProperty("publisher", new Part()
    {
      Id = _publisherOptions.Name
    });
    crate.RootDataset.SetProperty("datePublished", DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ssK"));

    // Add dataset and files contained within and update the CreateAction
    var createAction = crate.Entities.Values.First(x => x.GetProperty<string>("@type") == "CreateAction");
    crate.Add(outputs);
    foreach (var outputFile in outputFiles)
    {
      crate.Add(outputFile);
      createAction.AppendTo("result", outputFile);
    }

    crate.Save(location: metaDirInfo.FullName);
  }
}
