using HutchAgent.Config;
using HutchAgent.Constants;
using HutchAgent.Models;
using HutchAgent.Models.JobQueue;
using HutchAgent.Services;
using HutchAgent.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;

namespace HutchAgent.Controllers;

[ApiController]
[AllowAnonymous]
[Produces("application/json")]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
  private readonly WorkflowJobService _jobs;
  private readonly JobActionsQueueOptions _queueOptions;
  private readonly IQueueWriter _queueWriter;
  private readonly StatusReportingService _status;
  private readonly JobLifecycleService _job;


  public JobsController(
    IOptions<JobActionsQueueOptions> queueOptions,
    IQueueWriter queueWriter,
    WorkflowJobService jobs,
    StatusReportingService status,
    JobLifecycleService job)
  {
    _queueWriter = queueWriter;
    _jobs = jobs;
    _status = status;
    _job = job;
    _queueOptions = queueOptions.Value;
  }

  /// <summary>
  /// Submit a crate via POST Request Body for an already created job with the given id.
  /// </summary>
  /// <param name="id">The ID of the of the job to submit a crate for.</param>
  /// <param name="crate">A TRE-FX 5 Safes RO-Crate for the job.</param>
  [HttpPost("{id}/crate")]
  [Consumes("multipart/form-data")]
  [SwaggerResponse(202, "The Submitted Request Crate was accepted and queued to be executed.",
    typeof(JobStatusModel))]
  [SwaggerResponse(400, "The Request Crate was invalid.", typeof(List<string>))]
  [SwaggerResponse(404, "A Job with the provided ID could not be found.")]
  [SwaggerResponse(409, "This Job already has a Request Crate submitted.")]
  public async Task<IActionResult> SubmitCrate(string id, IFormFile crate)
  {
    try
    {
      // First check the job exists from Hutch's perspective
      var job = await _jobs.Get(id);

      // If so, then check if we've already got a crate
      if (job.HasCrateSubmitted())
        return Conflict("This Job has already had a Crate submitted.");

      // Record this as the crate source for now to avoid repeat submissions
      job.CrateSource = crate.FileName;
      await _jobs.Set(job);

      // Now check we can get the file
      await using var stream = crate.OpenReadStream();
      if (stream is null)
      {
        // Reset Crate Source
        job.CrateSource = null;
        await _jobs.Set(job);

        throw new InvalidOperationException(
          $"Couldn't open a stream for the crate in Job {id}");
      }

      var result = _job.AcceptRequestCrate(job, stream);
      if (!result.Success)
      {
        // Reset Crate Source
        job.CrateSource = null;
        await _jobs.Set(job);

        // Get rid of anything we stored so far
        try
        {
          Directory.Delete(job.WorkingDirectory);
        }
        catch (DirectoryNotFoundException)
        {
          /* Success */
        }

        return BadRequest(result.Errors);
      }

      _queueWriter.SendMessage(
        _queueOptions.QueueName,
        new JobQueueMessage
        {
          JobId = job.Id,
          ActionType = JobActionTypes.Execute
        });

      await _status.ReportStatus(job.Id, JobStatus.Queued);

      return Accepted(new JobStatusModel
      {
        Id = job.Id,
        Status = JobStatus.Queued.ToString()
      });
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Provide a crate URL via POST Request Body for an already created job with the given id.
  /// </summary>
  /// <param name="id">The ID of the of the job to provide a crate URL for.</param>
  /// <param name="url">A URL to a TRE-FX 5 Safes RO-Crate for the job, which Hutch can retrieve with a GET Request.</param>
  [HttpPost("{id}")]
  public IActionResult ProvideCrateUrl(string id, [FromBody] string url)
  {
    return Accepted();
  }

  /// <summary>
  /// Creates a Job entry and, if provided with a Crate URL, queues the job for Crate fetching.
  /// </summary>
  [HttpPost]
  [SwaggerResponse(200, "The Job was registered, and is awaiting submission of a Request Crate.",
    typeof(JobStatusModel))]
  [SwaggerResponse(202, "The Job was registered, and the submitted Request Crate URL queued to be fetched.",
    typeof(JobStatusModel))]
  [SwaggerResponse(400, "The Job details submitted are invalid.")]
  [SwaggerResponse(409, "Hutch is already actively managing a Job with this Id.")]
  public async Task<ActionResult<JobStatusModel>> Submit(SubmitJobModel model)
  {
    if (!ModelState.IsValid) return BadRequest();

    // If Valid (so far), create an initial Job state record.
    try
    {
      await _jobs.Create(new()
      {
        Id = model.JobId,
        DataAccess = model.DataAccess,
        CrateSource = model.CrateUrl?.ToString()
      });
    }
    catch (DbUpdateException)
    {
      return Conflict();
    }

    // If we have a crate URL, we should queue a fetch of the crate.
    if (model.CrateUrl is not null)
    {
      _queueWriter.SendMessage(
        _queueOptions.QueueName,
        new JobQueueMessage
        {
          JobId = model.JobId,
          ActionType = JobActionTypes.FetchAndExecute,
        });

      await _status.ReportStatus(model.JobId, JobStatus.FetchingCrate);
      return Accepted(new JobStatusModel
      {
        Id = model.JobId,
        Status = JobStatus.FetchingCrate.ToString()
      });
    }

    // (otherwise, we expect a raw crate, or URL, to be submitted at a later time.)
    await _status.ReportStatus(model.JobId, JobStatus.WaitingForCrate);
    return Ok(new JobStatusModel
    {
      Id = model.JobId,
      Status = JobStatus.WaitingForCrate.ToString()
    });
  }
}
