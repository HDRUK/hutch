using HutchAgent.Config;
using HutchAgent.Constants;
using HutchAgent.Models.JobQueue;
using HutchAgent.Services.Contracts;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;

namespace HutchAgent.Services.ActionHandlers;

/// <summary>
/// This ActionHandler will Check if a WorkflowJob is ready to InitiateEgress and,
/// if so, will Initiate the Egress process with the TRE modules.
/// </summary>
public class InitiateEgressActionHandler : IActionHandler
{
  private readonly JobLifecycleService _job;
  private readonly ILogger<InitiateEgressActionHandler> _logger;
  private readonly IFeatureManager _features;
  private readonly WorkflowTriggerService _workflow;
  private readonly MinioStoreServiceFactory _storeFactory;
  private readonly IQueueWriter _queueWriter;
  private readonly WorkflowJobService _jobs;
  private readonly StatusReportingService _status;
  private readonly JobActionsQueueOptions _queueOptions;
  private readonly ControllerApiService _controller;

  public InitiateEgressActionHandler(
    ILogger<InitiateEgressActionHandler> logger,
    IFeatureManager features,
    WorkflowTriggerService workflow,
    MinioStoreServiceFactory storeFactory,
    IQueueWriter queueWriter,
    WorkflowJobService jobs,
    StatusReportingService status,
    JobLifecycleService job,
    IOptions<JobActionsQueueOptions> queueOptions,
    ControllerApiService controller)
  {
    _logger = logger;
    _features = features;
    _workflow = workflow;
    _storeFactory = storeFactory;
    _queueWriter = queueWriter;
    _jobs = jobs;
    _status = status;
    _job = job;
    _controller = controller;
    _queueOptions = queueOptions.Value;
  }

  public async Task HandleAction(string jobId)
  {
    // 1. Check if job ready
    var job = await _jobs.Get(jobId);

    var completionResult = await _workflow.HasCompleted(job.ExecutorRunId);

    if (!completionResult.IsComplete) // not ready; re-queue to check again later
    {
      var message = new JobQueueMessage { ActionType = JobActionTypes.InitiateEgress, JobId = job.Id };
      _queueWriter.SendMessage(_queueOptions.QueueName, message);
      return;
    }

    job = await _job.UpdateWithWorkflowCompletion(job, completionResult);

    // 2. Prepare outputs for egress checks
    await _status.ReportStatus(job.Id, JobStatus.PreparingOutputs);

    _workflow.UnpackOutputs(job.ExecutorRunId, job.WorkingDirectory.JobEgressOutputs());

    // 3. Get target bucket for egress checks
    var useDefaultStore = await _features.IsEnabledAsync(FeatureFlags.UsePreconfiguredStore) 
                          || await _features.IsEnabledAsync(FeatureFlags.StandaloneMode);
    var store = _storeFactory.Create(
      useDefaultStore
        ? null
        : await _controller.RequestEgressBucket(job.Id));

    await _status.ReportStatus(job.Id, JobStatus.DataOutRequested);

    // 4. Upload files to bucket
    if (!await store.StoreExists())
    {
      const string message = "Could not write to the results store: Store not found";
      _logger.LogCritical(message);
      throw new InvalidOperationException(message);
    }

    await UploadFiles(
      store,
      job.WorkingDirectory.JobEgressOutputs(),
      useDefaultStore ? job.Id : ""); // In the default store, it's a shared bucket; otherwise expect a per-job bucket
    
    // 5. Inform TRE that outputs are ready for checks
    
    // TODO should we update metadata here with the fact the check was started? (yes)
    
    await _controller.ConfirmOutputsTransferred(job.Id);
    await _status.ReportStatus(job.Id, JobStatus.TransferredForDataOut);
  }

  private async Task UploadFiles(MinioStoreService store, string sourcePath, string targetPrefix = "")
  {
    await UploadFiles(store, sourcePath, "", targetPrefix);
  }

  private async Task UploadFiles(MinioStoreService store, string sourceRoot, string sourceSubPath, string targetPrefix)
  {
    // We do a bunch of path shenanigans to ensure relative directory paths are maintained inside the bucket
    var sourcePath = Path.Combine(sourceRoot, sourceSubPath);
    var a = File.GetAttributes(sourcePath);
    if ((a & FileAttributes.Directory) == FileAttributes.Directory)
    {
      foreach (var entry in Directory.EnumerateFileSystemEntries(sourcePath))
      {
        if (!Path.EndsInDirectorySeparator(sourceRoot))
          sourceRoot += Path.DirectorySeparatorChar;
        var relativeSubPath = entry.Replace(sourceRoot, "");
        
        await UploadFiles(store, sourceRoot, relativeSubPath, targetPrefix);
      }
    }
    else
    {
      await store.WriteToStore(sourcePath, Path.Combine(targetPrefix, sourceSubPath));
    }
  }
}
