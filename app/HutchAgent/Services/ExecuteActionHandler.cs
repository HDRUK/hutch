using HutchAgent.Config;
using HutchAgent.Constants;
using HutchAgent.Models;

namespace HutchAgent.Services;

public class ExecuteActionHandler
{
  private readonly WorkflowFetchService _workflowFetchService;
  private readonly WorkflowTriggerService _workflowTriggerService;
  private readonly WorkflowJobService _workflowJobService;
  private readonly IQueueWriter _queueWriter;
  private readonly JobActionsQueueOptions _queueOptions;
  private readonly CrateService _crates;

  public ExecuteActionHandler(
    WorkflowFetchService workflowFetchService,
    WorkflowTriggerService workflowTriggerService,
    WorkflowJobService workflowJobService,
    IQueueWriter queueWriter,
    JobActionsQueueOptions queueOptions,
    CrateService crates)
  {
    _workflowFetchService = workflowFetchService;
    _workflowTriggerService = workflowTriggerService;
    _workflowJobService = workflowJobService;
    _queueWriter = queueWriter;
    _queueOptions = queueOptions;
    _crates = crates;
  }

  public async Task Execute(string messageJobId)
  {
    // Get job.
    var job = await _workflowJobService.Get(messageJobId);

    // Initialise RO-Crate 
    var roCrate = _crates.InitialiseCrate(job.WorkingDirectory.BagItPayloadPath());

    // Check AssessActions exist and are complete
    _crates.CheckAssessActions(roCrate);

    // Fetch workflow.
    roCrate = await _workflowFetchService.FetchWorkflowCrate(job, roCrate);

    // Execute workflow.
    await _workflowTriggerService.TriggerWfexs(job, roCrate);

    // Update the job action in the queue.
    _queueWriter.SendMessage(_queueOptions.QueueName, new JobQueueMessage()
    {
      JobId = job.Id,
      ActionType = JobActionTypes.Finalize
    });
  }
}
