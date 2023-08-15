using System.IO.Compression;
using System.Text.Json.Nodes;
using HutchAgent.Config;
using HutchAgent.Constants;
using Microsoft.Extensions.Options;
using ROCrates;
using ROCrates.Exceptions;
using ROCrates.Models;
using File = System.IO.File;

namespace HutchAgent.Services;

/// <summary>
/// This service merges an output RO-Crate back into its input RO-Crate.
/// </summary>
public class CrateService
{
  private readonly string _pathToOutputDir = Path.Combine("data", "outputs");
  private readonly PublisherOptions _publisherOptions;
  private readonly ILogger<CrateService> _logger;

  public CrateService(IOptions<PublisherOptions> publisher, ILogger<CrateService> logger)
  {
    _logger = logger;
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
  /// Initialise Crate
  /// </summary>
  /// <param name="cratePath">Path to RO-Crate</param>
  /// <returns>RO-Crate object</returns>
  public ROCrate InitialiseCrate(string cratePath)
  {
    var crate = new ROCrate();
    try
    {
      crate.Initialise(cratePath);
    }
    catch (CrateReadException e)
    {
      _logger.LogError(exception: e, "RO-Crate cannot be read, or is invalid.");
      throw;
    }
    catch (MetadataException e)
    {
      _logger.LogError(exception: e, "RO-Crate Metadata cannot be read, or is invalid.");
      throw;
    }

    return crate;
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
    var outputs = new Dataset(source: Path.GetRelativePath(metaDirInfo.FullName, outputsDirToAdd));

    var crate = new ROCrate();
    crate.Initialise(metaDirInfo.FullName);
    crate.RootDataset.AppendTo("hasPart", outputs);
    crate.RootDataset.SetProperty("publisher", new Part()
    {
      Id = _publisherOptions.Name
    });
    crate.RootDataset.SetProperty("datePublished", DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ssK"));
    crate.Save(location: metaDirInfo.FullName);
  }

  /// <summary>
  /// Update entity action status in an RO-Crate
  /// </summary>
  /// <param name="status">The status to update to</param>
  /// <param name="entity">The RO-Crate entity to update</param>
  public void UpdateCrateActionStatus(string status, Entity entity)
  {
    entity.SetProperty("actionStatus", new Part()
    {
      Id = status
    });
  }

  /// <summary>
  /// Get execute entity from RO-Crate
  /// </summary>
  /// <param name="roCrate">the RO-Crate to look through</param>
  /// <returns>The execute action entity</returns>
  public Entity GetExecuteEntity(ROCrate roCrate)
  {
    //Get execution details
    var mentions = roCrate.RootDataset.GetProperty<JsonArray>("mentions") ?? throw new NullReferenceException("No mentions found in RO-Crate RootDataset Properties");
    var mainEntity = roCrate.RootDataset.GetProperty<Part>("mainEntity") ?? throw new NullReferenceException();
    // Check entity type and instrument
    var executeEntityId = mentions.Where(mention => mention?["@id"] != null &&
                                                    roCrate.Entities[
                                                        mention["@id"]!.ToString() ??
                                                        throw new NullReferenceException()]
                                                      .Properties["@type"]!.ToString() == "CreateAction" &&
                                                    roCrate.Entities[mention["@id"]!.ToString()]
                                                      .Properties["instrument"]?["@id"]?.ToString() == mainEntity.Id)
      .ToArray();
    var executeAction = roCrate.Entities[executeEntityId.First()?["@id"]!.ToString() ?? throw new InvalidOperationException($"No entity found with id of {executeEntityId.First()?["@id"]}")];
    return executeAction;
  }
  
  public Entity GetAssessAction(ROCrate roCrate, string actionType)
  {
    //Get execution details
    var mentions = roCrate.RootDataset.GetProperty<JsonArray>("mentions") ?? throw new NullReferenceException("No mentions found in RO-Crate RootDataset Properties");
    var mainEntity = roCrate.RootDataset.GetProperty<Part>("mainEntity") ?? throw new NullReferenceException();
    Entity assessAction = new Entity();
    // Get AssessAction by 'additionalType'
    switch (actionType)
    {
      case ActionType.CheckValueType:
      {
        var entityId = mentions.Where(mention =>
          mention != null && mention["@id"]?["@type"]?.ToString() == "AssessAction" &&
          mention["@id"]?["additionalType"]?["@id"]?.ToString() == ActionType.CheckValueType).Select(mention => mention?["@id"]?.ToString());
        assessAction = roCrate.Entities[entityId.First() ?? throw new InvalidOperationException()];
        break;
      }
      case ActionType.DisclosureCheck:
      {
        var entityId = mentions.Where(mention =>
          mention != null && mention["@id"]?["@type"]?.ToString() == "AssessAction" &&
          mention["@id"]?["additionalType"]?["@id"]?.ToString() == ActionType.DisclosureCheck).Select(mention => mention?["@id"]?.ToString());
        assessAction = roCrate.Entities[entityId.First() ?? throw new InvalidOperationException()];
        break;
      }
      case ActionType.SignOff:
      {
        var entityId = mentions.Where(mention =>
          mention != null && mention["@id"]?["@type"]?.ToString() == "AssessAction" &&
          mention["@id"]?["additionalType"]?["@id"]?.ToString() == ActionType.SignOff).Select(mention => mention?["@id"]?.ToString());
        assessAction = roCrate.Entities[entityId.First() ?? throw new InvalidOperationException()];
        break;
      }
      case ActionType.ValidationCheck:
      {
        var entityId = mentions.Where(mention =>
          mention != null && mention["@id"]?["@type"]?.ToString() == "AssessAction" &&
          mention["@id"]?["additionalType"]?["@id"]?.ToString() == ActionType.ValidationCheck).Select(mention => mention?["@id"]?.ToString());
        assessAction = roCrate.Entities[entityId.First() ?? throw new InvalidOperationException()];
        break;
      }
      case ActionType.GenerateCheckValue:
      {
        var entityId = mentions.Where(mention =>
          mention != null && mention["@id"]?["@type"]?.ToString() == "AssessAction" &&
          mention["@id"]?["additionalType"]?["@id"]?.ToString() == ActionType.GenerateCheckValue).Select(mention => mention?["@id"]?.ToString());
        assessAction = roCrate.Entities[entityId.First() ?? throw new InvalidOperationException()];
        break;
      }
    }
    
    return assessAction;
  }

  /// <summary>
  /// Create and add Disclosure Check entity to an RO-Crate.
  /// </summary>
  /// <param name="roCrate">The RO-Crate to add to.</param>
  public void CreateDisclosureCheck(ROCrate roCrate)
  {
    // Create unique id
    var disclosureCheckId = $"#disclosure-{Guid.NewGuid()}";
    // Add new ContextEntity to RO-Crate
    var disclosureCheck = new ContextEntity(roCrate, disclosureCheckId);
    roCrate.Add(disclosureCheck);
    
    // Set Properties
    disclosureCheck.SetProperty("@type", "AssessAction");
    disclosureCheck.SetProperty("additionalType", new Part()
    {
      Id = ActionType.DisclosureCheck
    });
    // Need to calculate estimate from startTime
    disclosureCheck.SetProperty("startTime", DateTime.Now);
    disclosureCheck.SetProperty("name", "Disclosure check of workflow results: pending (estimate: 1 week)");
    disclosureCheck.SetProperty("object", new Part()
    {
      Id = roCrate.RootDataset.Id
    });
    roCrate.RootDataset.GetProperty<JsonArray>("mentions");
    // Get sign off action agent
    var signOffAction = GetAssessAction(roCrate, ActionType.SignOff);
    var agent = signOffAction.GetProperty<Part>("agent") ?? throw new InvalidOperationException();
    disclosureCheck.SetProperty("agent", new Part()
    {
      Id = agent.Id
    });
    disclosureCheck.SetProperty("actionStatus", ActionStatus.ActiveActionStatus);
    // Append to RootDataset mentions
    roCrate.RootDataset.AppendTo("mentions", disclosureCheck);
  }

  /// <summary>
  /// Delete container images
  /// </summary>
  /// <param name="pathToImagesDir">The path to container images directory.</param>
  public void DeleteContainerImages(string pathToImagesDir)
  {
    Directory.Delete(pathToImagesDir, recursive: true);
  }
}
