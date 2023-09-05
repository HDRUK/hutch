using System.IO.Compression;
using System.Text.RegularExpressions;
using HutchAgent.Constants;
using HutchAgent.Data.Entities;
using ROCrates;
using ROCrates.Models;
using File = System.IO.File;

namespace HutchAgent.Services;

public class WorkflowFetchingService
{
  private readonly CrateService _crates;
  private readonly ILogger<WorkflowFetchingService> _logger;

  public WorkflowFetchingService(
    CrateService crates,
    ILogger<WorkflowFetchingService> logger)
  {
    _crates = crates;
    _logger = logger;
  }

  /// <summary>
  /// Fetch workflow specified in RO-Crate mainEntity
  /// </summary>
  /// <param name="workflowJob"></param>
  /// <returns></returns>
  /// <exception cref="Exception"></exception>
  /// <exception cref="InvalidOperationException"></exception>
  public async Task<ROCrate> FetchWorkflow(WorkflowJob workflowJob)
  {
    var roCrate = _crates.InitialiseCrate(workflowJob.WorkingDirectory.BagItPayloadPath());
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
      var clientStream = await client.GetStreamAsync(downloadAddress) ??
                         throw new InvalidOperationException("Invalid download URI");

      await using var file =
        File.OpenWrite(Path.Combine(workflowJob.WorkingDirectory.BagItPayloadPath(), "workflows.zip"));
      await clientStream.CopyToAsync(file);
      _logger.LogInformation("Successfully downloaded workflow from Workflow Hub.");
    }

    //Set DownloadAction status to Completed
    _crates.UpdateCrateActionStatus(ActionStatus.CompletedActionStatus, downloadAction);

    downloadAction.SetProperty("endTime", DateTime.Now);
    downloadAction.SetProperty("result", new Part()
    {
      Id = Path.Combine("workflow", workflowId)
    });
    using (var archive =
           new ZipArchive(File.OpenRead(Path.Combine(workflowJob.WorkingDirectory.BagItPayloadPath(),
             "workflows.zip"))))
    {
      Directory.CreateDirectory(Path.Combine(workflowJob.WorkingDirectory.BagItPayloadPath(), "workflow", workflowId));
      archive.ExtractToDirectory(Path.Combine(workflowJob.WorkingDirectory.BagItPayloadPath(), "workflow", workflowId));
      _logger.LogInformation(
        $"Unpacked workflow to {Path.Combine(workflowJob.WorkingDirectory.BagItPayloadPath(), "workflow", workflowId)}");
    }

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
    roCrate.Save(workflowJob.WorkingDirectory.BagItPayloadPath());
    _logger.LogInformation($"Saved updated RO-Crate to {workflowJob.WorkingDirectory.BagItPayloadPath()}.");

    return roCrate;
  }

  private static Entity CreateDownloadAction(ROCrate roCrate, string downloadAddress)
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
