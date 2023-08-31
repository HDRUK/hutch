using HutchAgent.Models;
using HutchAgent.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ROCrates.Exceptions;

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
    
    await using var stream = model.Crate.OpenReadStream();

    // Unpack the crate
    string? bagitPath;
    {
      if (stream is null)
        throw new InvalidOperationException(
          $"Couldn't open a stream for the crate in Job {model.JobId}");

      bagitPath = _crates.UnpackJobCrate(model.JobId, stream);
    }

    // Validate the Crate
    try
    {
      // TODO: BagIt checksum validation? or do this during execution?

      // Validate that it's a crate at all, by trying to Initialise it
      _crates.InitialiseCrate(bagitPath.BagItPayloadPath());

      // TODO: 5 safes crate profile validation? or do this during execution?
    }
    catch (Exception e) when (e is CrateReadException || e is MetadataException)
    {
      return BadRequest("Crate Payload is not an RO-Crate.");
    }

    // If Valid, Queue the job for an execution attempt

    //await _workflowTriggerService.TriggerWfexs(sr);
    return Accepted();
  }
}
