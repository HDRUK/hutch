using HutchAgent.Config;
using HutchAgent.Constants;
using HutchAgent.Models;
using HutchAgent.Models.JobQueue;
using HutchAgent.Services;
using HutchAgent.Services.Contracts;
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
  private readonly StatusReportingService _status;


  public JobsController(
    IOptions<JobActionsQueueOptions> queueOptions,
    CrateService crates,
    IQueueWriter queueWriter,
    WorkflowJobService jobs,
    StatusReportingService status)
  {
    _crates = crates;
    _queueWriter = queueWriter;
    _jobs = jobs;
    _status = status;
    _queueOptions = queueOptions.Value;
  }

  [HttpPost]
  public async Task<IActionResult> Submit(SubmitJobModel model)
  {
    if (ModelState.IsValid) return BadRequest();
    if (model.OutputStore.Url is null) return BadRequest();

    if (model.Crate is null) return BadRequest(); // TODO: Support fetching crate from remote URL
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
      try
      {
        Directory.Delete(bagitPath, recursive: true);
      }
      catch (DirectoryNotFoundException)
      {
        /* Success! */
      }

      return BadRequest("Crate Payload is not an RO-Crate.");
    }

    // If Valid (so far), Queue the job for an execution attempt
  /// <summary>
  /// Creates a Job entry and, if provided with a Crate URL, queues the job for Crate fetching.
  /// </summary>
  [HttpPost]
  public async Task<ActionResult<JobStatusModel>> Submit(SubmitJobModel model)
  {
    if (!ModelState.IsValid) return BadRequest();

    // If Valid (so far), create an initial Job state record.
    await _jobs.Create(new()
    {
      Id = model.JobId,
      DataAccess = model.DataAccess
    });

    // If we have a crate URL, we should queue a fetch of the crate.
    if (model.CrateUrl is not null)
    {
      _queueWriter.SendMessage(
        _queueOptions.QueueName,
        new JobQueueMessage<FetchAndExecutePayload>
        {
          JobId = model.JobId,
          ActionType = JobActionTypes.FetchAndExecute,
          Payload = new()
          {
            CrateUrl = model.CrateUrl.ToString()
          }
        });

      _status.ReportStatus(model.JobId, JobStatus.FetchingCrate);
      return Accepted(new JobStatusModel
      {
        Id = model.JobId,
        Status = JobStatus.FetchingCrate.ToString()
      });
    }

    // (otherwise, we expect a raw crate, or URL, to be submitted at a later time.)
    _status.ReportStatus(model.JobId, JobStatus.WaitingForCrate);
    return Ok(new JobStatusModel
    {
      Id = model.JobId,
      Status = JobStatus.WaitingForCrate.ToString()
    });
  }
}
