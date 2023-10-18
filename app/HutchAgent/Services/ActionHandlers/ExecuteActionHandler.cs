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
  private readonly FiveSafesCrateService _crates;

  public ExecuteActionHandler(
    WorkflowFetchService workflowFetchService,
    WorkflowTriggerService workflowTriggerService,
    WorkflowJobService workflowJobService,
    IQueueWriter queueWriter,
    IOptions<JobActionsQueueOptions> queueOptions,
    FiveSafesCrateService crates)
  {
    _workflowFetchService = workflowFetchService;
    _workflowTriggerService = workflowTriggerService;
    _workflowJobService = workflowJobService;
    _queueWriter = queueWriter;
    _queueOptions = queueOptions.Value;
    _crates = crates;
  }

  public async Task HandleAction(string jobId)
  {
    // Get job.
    var job = await _workflowJobService.Get(jobId);

    // Initialise RO-Crate 
    var roCrate = _crates.InitialiseCrate(job.WorkingDirectory.JobBagItRoot().BagItPayloadPath());

    // Check AssessActions exist and are complete
    _crates.CheckAssessActions(roCrate);
    
    // Check if Workflow RO-Crate URL is relative path
    if (!_crates.WorkflowIsRelativePath(roCrate, job))
    {
      // Fetch workflow.
      roCrate = await _workflowFetchService.FetchWorkflowCrate(job, roCrate);
    }
    
    // Execute workflow.
    await _workflowTriggerService.TriggerWfexs(job, roCrate);

    // Update the job action in the queue.
    _queueWriter.SendMessage(_queueOptions.QueueName, new JobQueueMessage()
    {
      JobId = job.Id,
      ActionType = JobActionTypes.InitiateEgress
    });
  }
}
