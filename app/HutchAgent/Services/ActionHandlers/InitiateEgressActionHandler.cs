using System.Text.Json;
using HutchAgent.Config;
using HutchAgent.Constants;
using HutchAgent.Data;
using HutchAgent.Models.JobQueue;
using HutchAgent.Services.Contracts;
using Microsoft.Extensions.Options;

namespace HutchAgent.Services.ActionHandlers;

/// <summary>
/// This ActionHandler will Check if a WorkflowJob is ready to InitiateEgress and,
/// if so, will Initiate the Egress process with the TRE modules.
/// </summary>
public class InitiateEgressActionHandler : IActionHandler
{
  private readonly JobLifecycleService _job;
  private readonly WorkflowTriggerService _workflow;
  private readonly IQueueWriter _queueWriter;
  private readonly WorkflowJobService _jobs;
  private readonly StatusReportingService _status;
  private readonly JobActionsQueueOptions _queueOptions;

  public InitiateEgressActionHandler(
    WorkflowTriggerService workflow,
    IQueueWriter queueWriter,
    WorkflowJobService jobs,
    StatusReportingService status,
    JobLifecycleService job,
    IOptions<JobActionsQueueOptions> queueOptions)
  {
    _workflow = workflow;
    _queueWriter = queueWriter;
    _jobs = jobs;
    _status = status;
    _job = job;
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

    // 4. Upload files to bucket

    // 5. Inform TRE that outputs are ready for checks
  }
}
