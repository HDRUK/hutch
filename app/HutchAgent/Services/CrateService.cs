using System.Globalization;
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
/// This service is for Hutch specific actions taken with RO-Crates 😊
/// </summary>
public class CrateService
{
  private readonly string _pathToOutputDir = Path.Combine("data", "outputs");
  private readonly PublisherOptions _publisherOptions;
  private readonly PathOptions _paths;
  private readonly LicenseOptions _license;
  private readonly ILogger<CrateService> _logger;

  public CrateService(
    IOptions<PathOptions> paths,
    IOptions<PublisherOptions> publisher,
    ILogger<CrateService> logger,
    IOptions<LicenseOptions> license)
  {
    _logger = logger;
    _license = license.Value;
    _publisherOptions = publisher.Value;
    _paths = paths.Value;
  }

  /// <summary>
  /// Create an RO-Crate object based on a RO-Crate saved on disk
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
      _logger.LogError(exception: e, "RO-Crate cannot be read, or is invalid");
      throw;
    }
    catch (MetadataException e)
    {
      _logger.LogError(exception: e, "RO-Crate Metadata cannot be read, or is invalid");
      throw;
    }

    return crate;
  }

// TODO not sure this is useful anymore?
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
  [Obsolete("TBC?")]
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
  /// <param name="job"></param>
  /// <exception cref="FileNotFoundException">
  /// Metadata file could not be found.
  /// </exception>
  /// <exception cref="InvalidDataException">The metadata file is invalid.</exception>
  public void UpdateMetadata(string pathToMetadata, Models.WorkflowJob job)
  {
    if (!File.Exists(Path.Combine(pathToMetadata, "ro-crate-metadata.json")))
      throw new FileNotFoundException("Could not locate the metadata for the RO-Crate.");

    var metaDirInfo = new DirectoryInfo(pathToMetadata);

    var outputsDirToAdd = Path.Combine(metaDirInfo.FullName, "outputs");
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
    createAction.SetProperty("started", job.ExecutionStartTime?.ToString(CultureInfo.InvariantCulture));
    createAction.SetProperty("ended", job.EndTime?.ToString(CultureInfo.InvariantCulture));
    crate.Add(outputs);
    foreach (var outputFile in outputFiles)
    {
      crate.Add(outputFile);
      createAction.AppendTo("result", outputFile);
    }

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
    _logger.LogInformation("Set {EntityId} actionStatus to {Status}", entity.Id, status);
  }

  /// <summary>
  /// Get execute entity from RO-Crate
  /// </summary>
  /// <param name="roCrate">the RO-Crate to look through</param>
  /// <returns>The execute action entity</returns>
  public Entity GetExecuteEntity(ROCrate roCrate)
  {
    //Get execution details
    var mentions = roCrate.RootDataset.GetProperty<JsonArray>("mentions") ??
                   throw new NullReferenceException("No mentions found in RO-Crate RootDataset Properties");
    var mainEntity = roCrate.RootDataset.GetProperty<Part>("mainEntity") ??
                     throw new NullReferenceException("No mainEntity found in RootDataset properties");
    // Check entity type is CreateAction
    var createActionEntities = mentions.Where(mention => mention?["@id"] != null &&
                                                         roCrate.Entities[
                                                             mention["@id"]!.ToString() ??
                                                             throw new NullReferenceException()]
                                                           .Properties["@type"]!.ToString() == "CreateAction");
    // Check entity instrument is the mainEntity ID
    var executeEntityId = createActionEntities.Where(mention => mention?["@id"] != null &&
                                                                roCrate.Entities[mention["@id"]!.ToString()]
                                                                  .Properties["instrument"]?["@id"]?.ToString() ==
                                                                mainEntity.Id).ToArray();
    if (executeEntityId is null) throw new NullReferenceException("No query action found in RootDataset mentions");
    if (executeEntityId.Length > 1) throw new Exception("More than one execute action found in RO-Crate");
    var executeAction =
      roCrate.Entities[
        executeEntityId.First()?["@id"]!.ToString() ??
        throw new InvalidOperationException($"No entity found with id of {executeEntityId.First()?["@id"]}")];
    _logger.LogInformation("Retrieved execution details from RO-Crate");
    return executeAction;
  }

  private Entity GetAssessAction(ROCrate roCrate, string actionType)
  {
    //Get execution details
    var mentions = roCrate.RootDataset.GetProperty<JsonArray>("mentions") ??
                   throw new NullReferenceException("No mentions found in RO-Crate RootDataset Properties");
    var assessActions = mentions.Where(mention =>
      mention != null &&
      roCrate.Entities[mention["@id"]!.ToString()].Properties["@type"]?.ToString() == "AssessAction");
    // Get AssessAction by 'additionalType'
    switch (actionType)
    {
      case ActionType.CheckValueType:
      {
        assessActions = assessActions.Where(mention =>
          roCrate.Entities[mention!["@id"]!.ToString()].Properties["additionalType"]?["@id"]?.ToString() ==
          ActionType.CheckValueType);
        break;
      }
      case ActionType.DisclosureCheck:
      {
        assessActions = assessActions.Where(mention =>
          roCrate.Entities[mention!["@id"]!.ToString()].Properties["additionalType"]?["@id"]?.ToString() ==
          ActionType.DisclosureCheck);
        break;
      }
      case ActionType.SignOff:
      {
        assessActions = assessActions.Where(mention =>
          roCrate.Entities[mention!["@id"]!.ToString()].Properties["additionalType"]?["@id"]?.ToString() ==
          ActionType.SignOff);
        break;
      }
      case ActionType.ValidationCheck:
      {
        assessActions = assessActions.Where(mention =>
          roCrate.Entities[mention!["@id"]!.ToString()].Properties["additionalType"]?["@id"]?.ToString() ==
          ActionType.ValidationCheck);
        break;
      }
      case ActionType.GenerateCheckValue:
      {
        assessActions = assessActions.Where(mention =>
          roCrate.Entities[mention!["@id"]!.ToString()].Properties["additionalType"]?["@id"]?.ToString() ==
          ActionType.GenerateCheckValue);
        break;
      }
    }

    var action = assessActions.Select(action => action?["@id"]) ??
                 throw new NullReferenceException("No assessAction found with the given id");
    var assessAction = roCrate.Entities[action.First()!.ToString()];
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

  public string GetStageFileName(ROCrate roCrate)
  {
    var parts = roCrate.RootDataset.GetProperty<JsonArray>("hasPart") ??
                throw new NullReferenceException("No property hasPart found in RO-Crate");
    var stageFile = parts.Where(part => part!["@id"]!.ToString().EndsWith(".stage")).ToList() ?? throw
      new InvalidOperationException();
    if (stageFile.Count > 1)
    {
      throw new Exception("More than one stage file referenced in RO-Crate");
    }

    if (stageFile.First() is null) throw new NullReferenceException("No stage file reference in RO-Crate");
    return stageFile.First()!["@id"]!.ToString();
  }

  public void CheckAssessActions(ROCrate roCrate)
  {
    //Check CheckValueType AssessAction exists and is Completed
    var checkValue = GetAssessAction(roCrate, ActionType.CheckValueType);
    if (checkValue is null)
      throw new Exception("Could not find CheckValue AssessAction in RO-Crate");

    var checkValueStatus = checkValue.GetProperty<JsonNode>("actionStatus");
    if (checkValueStatus?.ToString() is not (ActionStatus.CompletedActionStatus))
    {
      throw new Exception("CheckValue action status is null or not completed");
    }

    //Check ValidationCheck AssessAction exists and is Completed
    var validationCheck = GetAssessAction(roCrate, ActionType.ValidationCheck);
    if (validationCheck is null)
      throw new Exception("Could not find Validation check AssessAction in RO-Crate");

    var validationCheckStatus = validationCheck.GetProperty<JsonNode>("actionStatus");
    if (validationCheckStatus?.ToString() is not ActionStatus.CompletedActionStatus)
    {
      throw new Exception("Validation action status is null or not completed");
    }

    //Check SignOff AssessAction exists and is Completed
    var signOff = GetAssessAction(roCrate, ActionType.SignOff);
    if (signOff is null)
      throw new Exception("Could not find Sign Off AssessAction in RO-Crate");

    var signOffStatus = signOff.GetProperty<JsonNode>("actionStatus");
    if (signOffStatus?.ToString() is not ActionStatus.CompletedActionStatus)
    {
      throw new Exception("Sign Off action status is null or not completed");
    }
  }

  /// <summary>
  /// Update an RO-Crate's metadata file to include a license configured by Hutch.
  /// </summary>
  /// <param name="pathToCrate">The the path to the RO-Crate</param>
  /// <exception cref="FileNotFoundException">Thrown when the metadata file does not exist.</exception>
  public void AddLicense(string pathToCrate)
  {
    if (!File.Exists(Path.Combine(pathToCrate, "ro-crate-metadata.json")))
      throw new FileNotFoundException("Could not locate the metadata for the RO-Crate.");

    var license = new CreativeWork(
      identifier: _license.Uri,
      properties: _license.Properties);

    // Bug in ROCrates.Net: CreativeWork class uses the base constructor so @type is Thing by default
    license.SetProperty("@type", "CreativeWork");

    var crate = InitialiseCrate(pathToCrate);
    crate.Add(license);
    crate.RootDataset.SetProperty("license", new Part { Id = license.Id });
    crate.Save(location: pathToCrate);
  }
}
