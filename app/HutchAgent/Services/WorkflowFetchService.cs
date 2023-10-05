using System.IO.Compression;
using System.Text.RegularExpressions;
using HutchAgent.Constants;
using HutchAgent.Models;
using ROCrates;
using ROCrates.Exceptions;
using ROCrates.Models;
using File = System.IO.File;

namespace HutchAgent.Services;

public class WorkflowFetchService
{
  private readonly FiveSafesCrateService _crates;
  private readonly ILogger<WorkflowFetchService> _logger;
  private const string _workflowZip = "workflows.zip";

  public WorkflowFetchService(
    FiveSafesCrateService crates,
    ILogger<WorkflowFetchService> logger)
  {
    _crates = crates;
    _logger = logger;
  }

   /// <summary>
   /// Fetch workflow specified in RO-Crate mainEntity
   /// </summary>
   /// <param name="workflowJob"></param>
   /// <param name="roCrate"></param>
   /// <returns></returns>
   /// <exception cref="Exception"></exception>
  public async Task<ROCrate> FetchWorkflowCrate(WorkflowJob workflowJob,ROCrate roCrate)
  {
    // Get mainEntity from metadata, contains workflow location
    var mainEntity = roCrate.RootDataset.GetProperty<Part>("mainEntity");
    if (mainEntity is null) throw new Exception("mainEntity is not defined in the root dataset.");

    var workflowId = Regex.Match(mainEntity.Id, @"\d+").Value;
    // Compose download url for workflowHub
    var downloadAddress = Regex.Replace(mainEntity.Id, @"([0-9]+)(\?version=[0-9]+)?$", @"$1/ro_crate$2");

    var downloadAction = CreateDownloadAction(roCrate, downloadAddress);

    // Set DownloadAction status to Active
    _crates.UpdateCrateActionStatus(ActionStatus.ActiveActionStatus, downloadAction);

    using (var client = new HttpClient())
    {
      try
      {
        var clientStream = await client.GetStreamAsync(downloadAddress);
        await using var file =
          File.OpenWrite(Path.Combine(workflowJob.WorkingDirectory.JobBagItRoot().BagItPayloadPath(), _workflowZip));
        await clientStream.CopyToAsync(file);
      }
      catch (Exception e)
      {
        _logger.LogError(exception: e, "Could not download workflow for given address.");
        // Set ActionStatus to failed and save updated
        downloadAction.SetProperty("endTime", DateTime.Now);
        _crates.UpdateCrateActionStatus(ActionStatus.FailedActionStatus, downloadAction);
        roCrate.Save(workflowJob.WorkingDirectory.JobBagItRoot().BagItPayloadPath());
        throw;
      }
    }

    _logger.LogInformation("Successfully downloaded workflow.");
    var workflowCrateExtractPath =
      Path.Combine(workflowJob.WorkingDirectory.JobBagItRoot().BagItPayloadPath(), "workflow", workflowId);
    using (var archive =
           new ZipArchive(File.OpenRead(Path.Combine(workflowJob.WorkingDirectory.JobBagItRoot().BagItPayloadPath(),
             _workflowZip))))
    {
      Directory.CreateDirectory(workflowCrateExtractPath);
      archive.ExtractToDirectory(workflowCrateExtractPath);
      _logger.LogInformation(
        $"Unpacked workflow to {Path.Combine(workflowJob.WorkingDirectory.JobBagItRoot().BagItPayloadPath(), "workflow", workflowId)}");
    }

    // Validate the Crate
    try
    {
      // Validate that it's a crate at all, by trying to Initialise it
      _crates.InitialiseCrate(workflowCrateExtractPath);
    }
    catch (Exception e) when (e is CrateReadException || e is MetadataException)
    {
      _logger.LogError(message: "RO-Crate downloaded is not a valid RO-Crate");
      // Set ActionStatus to Failed and save updated
      downloadAction.SetProperty("endTime", DateTime.Now);
      _crates.UpdateCrateActionStatus(ActionStatus.FailedActionStatus, downloadAction);
      roCrate.Save(workflowJob.WorkingDirectory.JobBagItRoot().BagItPayloadPath());
      throw;
    }

    //Set DownloadAction status to Completed
    _crates.UpdateCrateActionStatus(ActionStatus.CompletedActionStatus, downloadAction);

    downloadAction.SetProperty("endTime", DateTime.Now);
    downloadAction.SetProperty("result", new Part()
    {
      Id = Path.Combine("workflow", workflowId)
    });

    var workflowEntity = new Entity(roCrate);
    workflowEntity.SetProperty("@id", Path.Combine("workflow", workflowId));
    var property = roCrate.Entities[mainEntity.Id];
    workflowEntity.SetProperty("sameAs", new Part()
    {
      Id = property.Id
    });
    workflowEntity.SetProperty("@type", property.Properties["@type"]);
    workflowEntity.SetProperty("name", property.Properties["name"]);
    workflowEntity.SetProperty("conformsTo", property.Properties["conformsTo"]);
    workflowEntity.SetProperty("distribution", property.Properties["distribution"]);
    roCrate.Add(workflowEntity);
    roCrate.RootDataset.AppendTo("mentions", downloadAction);
    roCrate.Save(workflowJob.WorkingDirectory.JobBagItRoot().BagItPayloadPath());
    _logger.LogInformation($"Saved updated RO-Crate to {workflowJob.WorkingDirectory.JobBagItRoot().BagItPayloadPath()}.");

    return roCrate;
  }

  private static ContextEntity CreateDownloadAction(ROCrate roCrate, string downloadAddress)
  {
    // Create DownloadAction ContextEntity
    var downloadActionId = $"#download-{Guid.NewGuid()}";
    var downloadAction = new ContextEntity(roCrate, downloadActionId);
    roCrate.Add(downloadAction);
    downloadAction.SetProperty("@type", "DownloadAction");
    downloadAction.SetProperty("name", "Downloaded workflow RO-Crate via proxy");
    downloadAction.SetProperty("startTime", DateTime.Now);

    downloadAction.SetProperty("object", new Part()
    {
      Id = downloadAddress
    });

    downloadAction.SetProperty("agent", new Part()
    {
      Id = "http://proxy.example.com/"
    });
    return downloadAction;
  }
}
