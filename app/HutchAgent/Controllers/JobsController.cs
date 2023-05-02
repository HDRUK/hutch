using HutchAgent.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HutchAgent.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
  readonly WorkflowTriggerService _workflowTriggerService;

  public JobsController(WorkflowTriggerService workflowTriggerService)
  {
    _workflowTriggerService = workflowTriggerService;
  }

  [HttpPost]
  public async Task<IActionResult> Unpack(IFormFile? job)
  {
    if (job == null)
      return BadRequest();
    try
    {
      await using var sr = job.OpenReadStream();

      {
        await _workflowTriggerService.TriggerWfexs(sr);
        return Accepted();
      }
    }
    catch
    {
      return BadRequest();
    }
  }
}
