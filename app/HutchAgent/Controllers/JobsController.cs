using System.Runtime.CompilerServices;
using HutchAgent.Config;
using HutchAgent.Constants;
using HutchAgent.Models;
using HutchAgent.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ROCrates.Exceptions;

namespace HutchAgent.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
  private readonly CrateService _crates;
  private readonly WorkflowJobService _jobs;
  private readonly JobActionsQueueOptions _queueOptions;
  private readonly IQueueWriter _queueWriter;

  public JobsController(
    IOptions<JobActionsQueueOptions> queueOptions,
    CrateService crates,
    IQueueWriter queueWriter,
    WorkflowJobService jobs)
  {
    _crates = crates;
    _queueWriter = queueWriter;
    _jobs = jobs;
    _queueOptions = queueOptions.Value;
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
      try { Directory.Delete(bagitPath, recursive: true); }
      catch (DirectoryNotFoundException) { /* Success! */ }

      return BadRequest("Crate Payload is not an RO-Crate.");
    }

    // If Valid (so far), Queue the job for an execution attempt
    await _jobs.Create(model.JobId, bagitPath);
    _queueWriter.SendMessage(_queueOptions.QueueName, new JobQueueMessage()
    {
      JobId = model.JobId,
      ActionType = JobActionTypes.Execute
    });

    return Accepted();
  }
}
