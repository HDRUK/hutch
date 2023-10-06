using System.Text.Json;
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
  private readonly PathOptions _paths;

  public JobsController(
    IOptions<JobActionsQueueOptions> queueOptions,
    IQueueWriter queueWriter,
    WorkflowJobService jobs,
    StatusReportingService status,
    JobLifecycleService job,
    IOptions<PathOptions> paths)
  {
    _queueWriter = queueWriter;
    _jobs = jobs;
    _status = status;
    _job = job;
    _paths = paths.Value;
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
      if (!result.IsSuccess)
      {
        // Reset Crate Source
        job.CrateSource = null;
        await _jobs.Set(job);

        // Get rid of anything we stored so far
        try
        {
          Directory.Delete(job.WorkingDirectory, recursive: true);
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
  /// Provide a Cloud Storage Crate Source via POST Request Body for an already created job with the given id.
  /// </summary>
  /// <param name="id">The ID of the of the job to provide a crate URL for.</param>
  /// <param name="source">An object detailing where to find a TRE-FX 5 Safes RO-Crate for the job,
  /// which Hutch can retrieve from a configured Cloud Storage Provider.</param>
  [HttpPost("{id}/crateSource")]
  [SwaggerResponse(202, "The Submitted Request Crate Source was accepted and queued to be fetched.",
    typeof(JobStatusModel))]
  [SwaggerResponse(400, "The provided details were invalid.")]
  [SwaggerResponse(404, "A Job with the provided ID could not be found.")]
  [SwaggerResponse(409, "This Job already has a Request Crate submitted.")]
  public async Task<IActionResult> ProvideCrateFromCloud(string id, FileStorageDetails source)
  {
    try
    {
      // First check the job exists from Hutch's perspective
      var job = await _jobs.Get(id);

      // If so, then check if we've already got a crate
      if (job.HasCrateSubmitted())
        return Conflict("This Job has already had a Crate submitted.");

      // Record this as the crate source
      job.CrateSource = JsonSerializer.Serialize(source);
      await _jobs.Set(job);

      // Queue for Fetching
      _queueWriter.SendMessage(
        _queueOptions.QueueName,
        new JobQueueMessage
        {
          JobId = id,
          ActionType = JobActionTypes.FetchAndExecute,
        });

      await _status.ReportStatus(id, JobStatus.FetchingCrate);
      return Accepted(new JobStatusModel
      {
        Id = id,
        Status = JobStatus.FetchingCrate.ToString()
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
  [HttpPost("{id}/crateUrl")]
  [SwaggerResponse(202, "The Submitted Request Crate URL was accepted and queued to be fetched.",
    typeof(JobStatusModel))]
  [SwaggerResponse(400, "The provided URL was invalid.")]
  [SwaggerResponse(404, "A Job with the provided ID could not be found.")]
  [SwaggerResponse(409, "This Job already has a Request Crate submitted.")]
  public async Task<IActionResult> ProvideCrateUrl(string id, [FromBody] string url)
  {
    try
    {
      // Check Crate URL
      if (!IsValidCrateUrl(new Uri(url)))
        return BadRequest($"Expected an HTTP(S) URL for crateUrl, but got: {url}");

      // First check the job exists from Hutch's perspective
      var job = await _jobs.Get(id);

      // If so, then check if we've already got a crate
      if (job.HasCrateSubmitted())
        return Conflict("This Job has already had a Crate submitted.");

      // Record this as the crate source
      job.CrateSource = url;
      await _jobs.Set(job);

      // Write Crate URL and Queue for Fetching
      _queueWriter.SendMessage(
        _queueOptions.QueueName,
        new JobQueueMessage
        {
          JobId = id,
          ActionType = JobActionTypes.FetchAndExecute,
        });

      await _status.ReportStatus(id, JobStatus.FetchingCrate);
      return Accepted(new JobStatusModel
      {
        Id = id,
        Status = JobStatus.FetchingCrate.ToString()
      });
    }
    catch (UriFormatException e)
    {
      return BadRequest($"The URL is badly formed: {e.Message}");
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Check whether a provided crateUrl is valid for Hutch's purposes.
  /// </summary>
  /// <param name="crateUrl">The URL to check.</param>
  /// <returns>Whether the provided URL is valid.</returns>
  private static bool IsValidCrateUrl(Uri crateUrl)
    => crateUrl.IsAbsoluteUri && new[] { "http", "https" }.Contains(crateUrl.Scheme);

  /// <summary>
  /// Creates a Job entry and, if provided with a remote Crate, queues the job for Crate fetching.
  /// </summary>
  [HttpPost]
  [SwaggerResponse(200, "The Job was registered, and is awaiting submission of a Request Crate.",
    typeof(JobStatusModel))]
  [SwaggerResponse(202, "The Job was registered, and the submitted Request Crate Source queued to be fetched.",
    typeof(JobStatusModel))]
  [SwaggerResponse(400, "The Job details submitted are invalid.")]
  [SwaggerResponse(409, "Hutch is already actively managing a Job with this Id.")]
  public async Task<ActionResult<JobStatusModel>> Submit(SubmitJobModel model)
  {
    if (!ModelState.IsValid) return BadRequest();

    // Check mutual exclusivity of remote crate details
    if (model.CrateUrl is not null && model.CrateSource is not null)
      return BadRequest($"Expected only one of {nameof(model.CrateSource)} and {nameof(model.CrateUrl)}");

    // Check Crate URL if it's not null
    if (model.CrateUrl is not null && !IsValidCrateUrl(model.CrateUrl))
      return BadRequest($"Expected an HTTP(S) URL for crateUrl, but got: {model.CrateUrl}");

    // If Valid (so far), create an initial Job state record.
    try
    {
      await _jobs.Create(new()
      {
        Id = model.JobId,
        DataAccess = JsonSerializer.Serialize(model.DataAccess),
        CrateSource = model.CrateUrl?.ToString()
                      ?? (model.CrateSource is not null
                        ? JsonSerializer.Serialize(model.CrateSource)
                        : null),
        WorkingDirectory = _paths.JobWorkingDirectory(model.JobId)
      });
    }
    catch (DbUpdateException)
    {
      return Conflict();
    }

    // If we have a crate URL or Source, we should queue a fetch of the crate.
    if (model.CrateUrl is not null || model.CrateSource is not null)
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

  /// <summary>
  /// Accept an approval outcome for a job. If a job with the specified ID is fully approved, queue it for finalization.
  /// Otherwise treat the job as failed.
  /// </summary>
  /// <param name="id">The ID of the job.</param>
  /// <param name="result">The outcome of the approval checks.</param>
  /// <returns></returns>
  [HttpPost("{id}/approval")]
  [SwaggerResponse(200, "The approval process completed successfully.")]
  [SwaggerResponse(404, "The job corresponding to the given ID doesn't exist.")]
  public async Task<IActionResult> Approval(string id, [FromBody] ApprovalResult result)
  {
    var jobStatus = new JobStatusModel()
    {
      Id = id,
      Status = ""
    };
    try
    {
      var job = await _jobs.Get(id);

      if (result.Status == ApprovalType.FullyApproved)
      {
        _job.DisclosureCheckCompleted(job);

        _queueWriter.SendMessage(_queueOptions.QueueName, new JobQueueMessage()
        {
          ActionType = JobActionTypes.Finalize,
          JobId = id
        });
        jobStatus.Status = JobStatus.PackagingApprovedResults.ToString();

        await _status.ReportStatus(id, JobStatus.PackagingApprovedResults);
      }
      else
      {
        // Todo: support partial approval
        // Only finalise and include the approved files.

        // Todo: when failed
        // record disclosure check as failed and finalise without outputs.

        // TODO: return some sort of job status


        jobStatus.Status = JobStatus.Failure.ToString();
        await _job.Cleanup(job);
      }

      return Ok(jobStatus);
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
}
