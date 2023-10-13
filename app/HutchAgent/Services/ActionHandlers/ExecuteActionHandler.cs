using HutchAgent.Config;
using HutchAgent.Constants;
using HutchAgent.Models.JobQueue;
using HutchAgent.Services.Contracts;
using Microsoft.Extensions.Options;

namespace HutchAgent.Services.ActionHandlers;

public class ExecuteActionHandler : IActionHandler
{
  private readonly WorkflowFetchService _workflowFetchService;
  private readonly WorkflowTriggerService _workflowTriggerService;
  private readonly WorkflowJobService _workflowJobService;
  private readonly IQueueWriter _queueWriter;
  private readonly JobActionsQueueOptions _queueOptions;
  private readonly WorkflowTriggerOptions _workflowOptions;
  private readonly FiveSafesCrateService _crates;
  private readonly StatusReportingService _status;

  public ExecuteActionHandler(
    WorkflowFetchService workflowFetchService,
    WorkflowTriggerService workflowTriggerService,
    WorkflowJobService workflowJobService,
    IQueueWriter queueWriter,
    IOptions<JobActionsQueueOptions> queueOptions,
    FiveSafesCrateService crates,
    StatusReportingService status,
    IOptions<WorkflowTriggerOptions> workflowOptions)
  {
    _workflowFetchService = workflowFetchService;
    _workflowTriggerService = workflowTriggerService;
    _workflowJobService = workflowJobService;
    _queueWriter = queueWriter;
    _queueOptions = queueOptions.Value;
    _crates = crates;
    _status = status;
    _workflowOptions = workflowOptions.Value;
  }

  public async Task HandleAction(string jobId)
  {
    // Get job.
    var job = await _workflowJobService.Get(jobId);

    await _status.ReportStatus(jobId, JobStatus.ValidatingCrate);

    // Initialise RO-Crate 
    var roCrate = _crates.InitialiseCrate(job.WorkingDirectory.JobCrateRoot());

    // Check AssessActions exist and are complete
    _crates.CheckAssessActions(roCrate);

    // Fetch workflow.
    await _status.ReportStatus(jobId, JobStatus.FetchingWorkflow);

    roCrate = await _workflowFetchService.FetchWorkflowCrate(job, roCrate);

    // Execute workflow.
    if (string.IsNullOrWhiteSpace(_workflowOptions.SkipExecutionUsingOutputFile))
      await _workflowTriggerService.TriggerWfexs(job, roCrate);

    // Update the job action in the queue.
    _queueWriter.SendMessage(_queueOptions.QueueName, new JobQueueMessage()
    {
      JobId = job.Id,
      ActionType = JobActionTypes.InitiateEgress
    });
  }
}
