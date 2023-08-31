using HutchAgent.Models;
using HutchAgent.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HutchAgent.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
  private readonly CrateService _crates;
  readonly WorkflowTriggerService _workflowTriggerService;

  public JobsController(
    CrateService crates,
    WorkflowTriggerService workflowTriggerService)
  {
    _crates = crates;
    _workflowTriggerService = workflowTriggerService;
  }

  [HttpPost]
  public async Task<IActionResult> Submit(SubmitJobModel model)
  {

    if (ModelState.IsValid) return BadRequest();

    // Unpack the crate
    await using var stream = model.Crate.OpenReadStream();
    {
      if (stream is null)
        throw new InvalidOperationException(
          $"Couldn't open a stream for the crate in Job {model.JobId}");

      var bagitPath = _crates.UnpackJobCrate(model.JobId, stream);
    }

    // Validate it

    // If Valid, Queue the job for an execution attempt

      //await _workflowTriggerService.TriggerWfexs(sr);
    return Accepted();
  }
}
