using System.Text.Json;
using HutchAgent.Config;
using HutchAgent.Constants;
using HutchAgent.Models.JobQueue;
using HutchAgent.Results;
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

  public async Task HandleAction(string jobId, object? payloadObject)
  {
    var payload = (InitiateEgressPayloadModel?)payloadObject;

    // 1. Check if job ready
    _logger.LogInformation("Checking job status for job: {JobId}", jobId);
    var job = await _jobs.Get(jobId);

    if (!string.IsNullOrWhiteSpace(payload?.OutputFile))
    {
      _logger.LogInformation(
        "Job [{JobId}] OutputFile specified - Execution was skipped and Egress will be performed using the file at {OutputPath}",
        jobId,
        payload.OutputFile);
    }

    var completionResult = string.IsNullOrWhiteSpace(payload?.OutputFile)
      ? await _workflow.HasCompleted(job.ExecutorRunId)
      // if we have an output file, we skipped execution, so falsify the completion
      : new WorkflowCompletionResult
      {
        IsComplete = true,
        ExitCode = 0,
        StartTime = DateTimeOffset.UtcNow - TimeSpan.FromMinutes(1),
        EndTime = DateTimeOffset.UtcNow
      };

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

    // Unpack outputs from the appropriate source
    if (!string.IsNullOrWhiteSpace(payload?.OutputFile))
    {
      _logger.LogInformation("Job [{JobId}] Unpacking outputs directly from {OutputPath}", jobId, payload.OutputFile);
      _workflow.UnpackOutputsFromPath(payload.OutputFile, job.WorkingDirectory.JobEgressOutputs());
    }
    else
    {
      _logger.LogInformation(
        "Job [{JobId}] Unpacking outputs from Executor Run [{RunId}] working directory",
        jobId,
        job.ExecutorRunId);
      _workflow.UnpackOutputs(job.ExecutorRunId, job.WorkingDirectory.JobEgressOutputs());
    }

    // 3. Get target bucket for egress checks
    var useDefaultStore = await _features.IsEnabledAsync(FeatureFlags.StandaloneMode);
    var egressStore = useDefaultStore
      ? null
      : await _controller.RequestEgressBucket(job.Id);

    var store = await _storeFactory.Create(egressStore);

    // Record the bucket details for later use
    job.EgressTarget = JsonSerializer.Serialize(egressStore ?? _storeFactory.DefaultOptions);
    await _jobs.Set(job);

    _logger.LogInformation("job [{JobId}] Egress Target: {Target}", job.Id, job.EgressTarget);

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
    if (await _features.IsEnabledAsync(FeatureFlags.StandaloneMode))
    {
      _logger.LogInformation(
        "Egress outputs uploaded for manual inspection: Notify `/api/jobs/{JobId}/approval` when complete", jobId);
      files.ForEach(x => _logger.LogInformation("Output: {ObjectId}", x));
    }
    else
    {
      await _controller.ConfirmOutputsTransferred(job.Id, files);
      _logger.LogInformation("Job [{Jobid}]: TRE Controller notified of Data Transfer for Egress Inspection", jobId);
    }

    await _status.ReportStatus(job.Id, JobStatus.TransferredForDataOut);

    _job.DisclosureCheckInitiated(job);
  }
}
