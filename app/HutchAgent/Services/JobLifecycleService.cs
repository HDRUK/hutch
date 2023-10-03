using HutchAgent.Config;
using HutchAgent.Models;
using HutchAgent.Results;
using Microsoft.Extensions.Options;

namespace HutchAgent.Services;

public class JobLifecycleService
{
  private readonly RequestCrateService _requestCrates;
  private readonly WorkflowJobService _jobs;
  private readonly HttpClient _http;
  private readonly PathOptions _paths;

  public JobLifecycleService(
    RequestCrateService requestCrates,
    WorkflowJobService jobs,
    IHttpClientFactory httpClientFactory,
    IOptions<PathOptions> paths)
  {
    _requestCrates = requestCrates;
    _jobs = jobs;
    _paths = paths.Value;
    _http = httpClientFactory.CreateClient();
  }

  /// <summary>
  /// Fetch a remote Request Crate by making an HTTP GET Request to a provided URL.
  /// </summary>
  /// <param name="url">The URL to GET.</param>
  /// <returns>A <see cref="Stream"/> of the HTTP Response Body (hopefully an RO-Crate!)</returns>
  public async Task<Stream> FetchRemoteRequestCrate(string url) // TODO support Min.io as a crate Source
  {
    var response = await _http.GetAsync(url);
    response.EnsureSuccessStatusCode();

    return await response.Content.ReadAsStreamAsync();
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
    try
    {
      var bagitPath = _requestCrates.Unpack(job, crate);

      return _requestCrates.IsValidToAccept(bagitPath.BagItPayloadPath());
    }
    catch (InvalidDataException)
    {
      return new()
      {
        Errors = new() { "The provided file is not a valid zip archive." }
      };
    }
  }

  /// <summary>
  /// Update a WorkflowJob record with details pertaining to completion of its Workflow Execution
  /// </summary>
  /// <param name="job"></param>
  /// <param name="details"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentException"></exception>
  public async Task<WorkflowJob> UpdateWithWorkflowCompletion(WorkflowJob job, WorkflowCompletionResult details)
  {
    if (!details.IsComplete)
      throw new ArgumentException(
        "Expected workflow execution to be complete!", nameof(details));
    
    job.ExitCode = details.ExitCode;
    job.ExecutionStartTime = details.StartTime;
    job.EndTime = details.EndTime;

    return await _jobs.Set(job);
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
      Directory.Delete(job.WorkingDirectory, recursive: true);
    }
    catch (DirectoryNotFoundException)
    {
      /* Success */
    }
  }

  /// <summary>
  /// Clean up everything related to a job;
  /// its db record, its working directory etc.
  /// </summary>
  /// <param name="jobId">ID of the job to clean up</param>
  public async Task Cleanup(string jobId)
  {
    try
    {
      var job = await _jobs.Get(jobId);
      await Cleanup(job);
    }
    catch (KeyNotFoundException)
    {
      // DB record is already gone;
      // Try and remove the "expected" working directory since we don't know the actual
      try
      {
        Directory.Delete(_paths.JobWorkingDirectory(jobId), recursive: true);
      }
      catch (DirectoryNotFoundException)
      {
        /* Success */
      }
    }
  }
}
