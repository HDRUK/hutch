using System.Text.Json;
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
    _logger.LogInformation("Checking job status for job: {JobId}", jobId);
    var job = await _jobs.Get(jobId);

    var completionResult = await _workflow.HasCompleted(job.ExecutorRunId);

    if (!completionResult.IsComplete) // not ready; re-queue to check again later
    {
      _logger.LogInformation("Job [{JobId}] has not completed execution: re-queueing", jobId);
      var message = new JobQueueMessage { ActionType = JobActionTypes.InitiateEgress, JobId = job.Id };
      _queueWriter.SendMessage(_queueOptions.QueueName, message);
      return;
    }

    job = await _job.UpdateWithWorkflowCompletion(job, completionResult);
    _logger.LogInformation("Job [{JobId}] updated with execution complete", jobId);

    // 2. Prepare outputs for egress checks
    await _status.ReportStatus(job.Id, JobStatus.PreparingOutputs);

    _workflow.UnpackOutputs(job.ExecutorRunId, job.WorkingDirectory.JobEgressOutputs());

    // 3. Get target bucket for egress checks
    var useDefaultStore = await _features.IsEnabledAsync(FeatureFlags.UsePreconfiguredStore)
                          || await _features.IsEnabledAsync(FeatureFlags.StandaloneMode);
    var egressStore = useDefaultStore
      ? null
      : await _controller.RequestEgressBucket(job.Id);

    var store = _storeFactory.Create(egressStore);

    // Record the bucket details for later use
    job.EgressTarget = JsonSerializer.Serialize(egressStore ?? _storeFactory.DefaultOptions);
    await _jobs.Set(job);

    await _status.ReportStatus(job.Id, JobStatus.DataOutRequested);

    // 4. Upload files to bucket
    if (!await store.StoreExists())
    {
      const string message = "Could not write to the results store: Store not found";
      _logger.LogCritical(message);
      throw new InvalidOperationException(message);
    }

    var files = await store.UploadFilesRecursively(
      job.WorkingDirectory.JobEgressOutputs(),
      useDefaultStore ? job.Id : ""); // In the default store, it's a shared bucket; otherwise expect a per-job bucket

    // 5. Inform TRE that outputs are ready for checks
    // TODO should we update metadata here with the fact the check was started? (yes)
    if (await _features.IsEnabledAsync(FeatureFlags.StandaloneMode))
    {
      _logger.LogInformation(
        "Egress outputs uploaded for manual inspection: Notify `/api/jobs/{JobId}/approval` when complete", jobId);
    }
    else
    {
      await _controller.ConfirmOutputsTransferred(job.Id, files);
      _logger.LogInformation("Job [{Jobid}]: TRE Controller notified of Data Transfer for Egress Inspection", jobId);
    }
    
    await _status.ReportStatus(job.Id, JobStatus.TransferredForDataOut);
  }
}
