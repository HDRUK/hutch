using HutchAgent.Constants;
using HutchAgent.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;

namespace HutchAgent.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
  readonly WorkflowTriggerService _workflowTriggerService;
  readonly IFeatureManager _featureManager;

  public JobsController(WorkflowTriggerService workflowTriggerService, IFeatureManager featureManager)
  {
    _workflowTriggerService = workflowTriggerService;
    _featureManager = featureManager;
  }

  [HttpPost]
  public async Task<IActionResult> Unpack(IFormFile? job)
  {
    
    if (job == null)
      return BadRequest();
    await using var sr = job.OpenReadStream();
    {
      await _workflowTriggerService.TriggerWfexs(sr);
      return Accepted();
    }
  }
}
