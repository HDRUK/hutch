using System.Globalization;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Nodes;
using HutchAgent.Config;
using HutchAgent.Constants;
using HutchAgent.Models;
using HutchAgent.Results;
using Microsoft.Extensions.Options;
using ROCrates;
using ROCrates.Exceptions;
using ROCrates.Models;

namespace HutchAgent.Services;

/// <summary>
/// This service is for Hutch specific actions taken with RO-Crates 😊
/// </summary>
public class FiveSafesCrateService
{
  private readonly CratePublishingOptions _publishOptions;
  private readonly ILogger<FiveSafesCrateService> _logger;
  private readonly BagItService _bagIt;

  public FiveSafesCrateService(
    IOptions<PathOptions> paths,
    IOptions<CratePublishingOptions> publishOptions,
    ILogger<FiveSafesCrateService> logger,
    BagItService bagIt)
  {
    _logger = logger;
    _bagIt = bagIt;
    _publishOptions = publishOptions.Value;
  }

  /// <summary>
  /// Get the CreateAction Entity for a 5S RO-Crate.
  /// </summary>
  /// <param name="crate">The 5S Crate to find the CreateAction in.</param>
  /// <returns>The CreateAction Entity.</returns>
  public Entity
    GetCreateAction(ROCrate crate) // TODO some methods in here, like this one, could be extension methods on the Crate itself <3
  {
    return crate.Entities.Values.First(x => x.GetProperty<string>("@type") == "CreateAction");
  }

  /// <summary>
  /// Finalize a successful 5S Crate's metadata (The "Publishing Phase" of the spec) 
  /// </summary>
  /// <param name="job">Details of the job this working crate is for</param>
  public void FinalizeMetadata(WorkflowJob job)
  {
    var roCrateRootPath = job.WorkingDirectory.JobCrateRoot();
    var crate = InitialiseCrate(roCrateRootPath);

    // a) Add Outputs
    // TODO in future be more granular with outputs based on workflow definition?

    // i. the actual outputs entity(/ies)
    var outputs = new Dataset(source: "outputs");
    crate.Add(outputs);

    // ii. CreateAction results
    var createAction = GetCreateAction(crate);
    createAction.AppendTo("result", outputs);

    // iii. Root hasPart results
    crate.RootDataset.AppendTo("hasPart", outputs);

    // b) Add Licence and Publisher details
    if (_publishOptions.Publisher is not null)
      crate.RootDataset.SetProperty("publisher", new Part()
      {
        Id = _publishOptions.Publisher.Id
      });
    AddLicense(crate);


    // c) Root datePublished
    crate.RootDataset.SetProperty(
      "datePublished",
      DateTimeOffset.UtcNow.ToString("o", CultureInfo.InvariantCulture));

    crate.Save(roCrateRootPath);
  }

  /// <summary>
  /// Perform cursory validation of a 5S Crate submission;
  /// enough to know whether we accept it in principle to try and Execute later.
  /// </summary>
  /// <param name="cratePath">Path to an RO-Crate root (i.e. with metadata)</param>
  /// <returns>A <see cref="BasicResult"/> indicating the outcome of the validation.</returns>
  public BasicResult IsValidToAccept(string cratePath)
  {
    var result = new BasicResult();

    // throw invalid data if checksums don't match
    var bagItDir = new DirectoryInfo(cratePath).Parent?.FullName;
    if (bagItDir is not null && _bagIt.VerifyChecksums(bagItDir).Result)
    {
      // Validate that it's an RO-Crate at all, by trying to Initialise it
      try
      {
        InitialiseCrate(cratePath);
      }
      catch (Exception e) when (e is CrateReadException || e is MetadataException)
      {
        result.Errors.Add("The provided file is not an RO-Crate.");
      }
    }
    else
    {
      result.Errors.Add(
        "The files' checksums do not match. Check their contents, remake the checksums and re-submit the job.");
    }

    // TODO: 5 safes crate profile validation? or do this during execution?

    result.IsSuccess = true;
    return result;
  }

  /// <summary>
  /// Unzips a zipped 5 Safes RO-Crate inside a job's working directory.
  /// </summary>
  /// <param name="job">A model describing the job the crate is for.</param>
  /// <param name="crate">A stream of the crate's bytes.</param>
  /// <returns>The path where the crate was unpacked.</returns>
  public string Unpack(WorkflowJob job, Stream crate)
  {
    var targetPath = job.WorkingDirectory.JobBagItRoot();

    using var archive = new ZipArchive(crate);

    Directory.CreateDirectory(targetPath);
    archive.ExtractToDirectory(targetPath, overwriteFiles: true);

    _logger.LogInformation("Job [{JobId}] Crate extracted at {TargetPath}", job.Id, targetPath);

    return targetPath;
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
    _logger.LogDebug("Set {EntityId} actionStatus to {Status}", entity.Id, status);
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
    _logger.LogDebug("Retrieved execution details from RO-Crate");
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
  /// Add License details to a loaded 5S Results Crate.
  /// </summary>
  /// <param name="crate">The crate</param>
  public void AddLicense(ROCrate crate)
  {
    if (string.IsNullOrEmpty(_publishOptions.License?.Uri)) return;

    var licenseProps = _publishOptions.License.Properties;
    var licenseEntity = new CreativeWork(
      identifier: _publishOptions.License.Uri,
      properties: JsonSerializer.SerializeToNode(licenseProps)?.AsObject());

    // Bug in ROCrates.Net: CreativeWork class uses the base constructor so @type is Thing by default
    licenseEntity.SetProperty("@type", "CreativeWork");

    crate.Add(licenseEntity);

    crate.RootDataset.SetProperty("license", new Part { Id = licenseEntity.Id });
  }

  /// <summary>
  /// Check whether workflow path is relative and validate it exists
  /// </summary>
  /// <param name="roCrate"></param>
  /// <param name="workDir"></param>
  /// <returns></returns>
  /// <exception cref="NullReferenceException"></exception>
  public bool WorkflowIsRelativePath(ROCrate roCrate, string workDir)
  {
    var mainEntity = roCrate.RootDataset.GetProperty<Part>("mainEntity") ??
                     throw new NullReferenceException("No main entity found in RO-Crate");
    // Check if mainEntity is remote URL
    if (Uri.IsWellFormedUriString(mainEntity.Id, UriKind.Absolute))
    {
      return false;
    }

    var relPath = Path.Combine(workDir.JobCrateRoot(), mainEntity.Id);
    // Check relative path exists
    if (Path.Exists(relPath))
    {
      {
        // check it's a valid crate
        InitialiseCrate(relPath);
        return true;
      }
    }

    return false;
  }

  /// <summary>
  /// Mark a disclosure check as successfully completed.
  /// </summary>
  /// <param name="crate"></param>
  /// <param name="checkEndTime"></param>
  public void CompleteDisclosureCheck(ROCrate crate, DateTimeOffset checkEndTime)
  {
    // i. Amend AssessAction
    var disclosureAction = GetAssessAction(crate, ActionType.DisclosureCheck);
    UpdateCrateActionStatus(ActionStatus.CompletedActionStatus, disclosureAction);
    disclosureAction.SetProperty("endTime",
      checkEndTime.ToString("o", CultureInfo.InvariantCulture));
    disclosureAction.SetProperty("name",
      "Disclosure check of workflow results: Fully approved"); // TODO Partial approval some day
    // ii. Root mentions - already done when the AssessAction was created
  }
}
