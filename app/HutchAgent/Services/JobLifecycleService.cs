using HutchAgent.Models;
using HutchAgent.Results;

namespace HutchAgent.Services;

public class JobLifecycleService
{
  private readonly RequestCrateService _requestCrates;
  private readonly WorkflowJobService _jobs;

  public JobLifecycleService(
    RequestCrateService requestCrates,
    WorkflowJobService jobs)
  {
    _requestCrates = requestCrates;
    _jobs = jobs;
  }

  /// <summary>
  /// Try and accept an acquired Job Crate,
  /// by unpacking it, and doing some cursory validation.
  /// </summary>
  /// <param name="job">A model describing the Job the Crate is for.</param>
  /// <param name="crate">A stream of the Crate's bytes.</param>
  /// <returns>A <see cref="BasicResult"/> indicating whether the Crate was accepted, and if not why not.</returns>
  public BasicResult AcceptRequestCrate(WorkflowJob job, Stream crate)
  {
    var bagitPath = _requestCrates.Unpack(job, crate);

    return _requestCrates.IsValidToAccept(bagitPath.BagItPayloadPath());
  }

  /// <summary>
  /// Clean up everything related to a job;
  /// its db record, its working directory etc.
  /// </summary>
  /// <param name="job">A model describing the job to clean up</param>
  public async Task Cleanup(WorkflowJob job)
  {
    await _jobs.Delete(job.Id);
    
    try
    {
      Directory.Delete(job.WorkingDirectory);
    } catch(DirectoryNotFoundException) { /* Success */ }
  }
}
