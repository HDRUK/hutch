using System.Text.Json;
using HutchAgent.Constants;
using HutchAgent.Models;
using HutchAgent.Services.Contracts;
using Microsoft.FeatureManagement;

namespace HutchAgent.Services.ActionHandlers;

/// <summary>
/// This ActionHandler will Fetch the Request Crate for a WorkflowJob from a remote URL,
/// and attempt to Execute it if it passes validation.
/// </summary>
public class FetchAndExecuteActionHandler : IActionHandler
{
  private readonly ExecuteActionHandler _executeHandler;
  private readonly JobLifecycleService _job;
  private readonly WorkflowJobService _jobs;
  private readonly StatusReportingService _status;
  private readonly MinioStoreService _store;
  private readonly IFeatureManager _features;

  public FetchAndExecuteActionHandler(
    ExecuteActionHandler executeHandler,
    WorkflowJobService jobs,
    StatusReportingService status,
    JobLifecycleService job,
    MinioStoreService store,
    IFeatureManager features)
  {
    _executeHandler = executeHandler;
    _jobs = jobs;
    _status = status;
    _job = job;
    _store = store;
    _features = features;
  }

  public async Task HandleAction(string jobId)
  {
    try
    {
      var job = await _jobs.Get(jobId);

      if (string.IsNullOrWhiteSpace(job.CrateSource))
      {
        await _status.ReportStatus(jobId, JobStatus.Failure,
          $"The remote crate could not be fetched from the provided source: {job.CrateSource}");

        await _job.Cleanup(job);
        return;
      }

      // Fetch
      var crateUrl = job.CrateSource;
      try
      {
        var cloudCrate = JsonSerializer.Deserialize<FileStorageDetails>(job.CrateSource);

        if (cloudCrate is not null)
        {
          // If the details deserialised successfully, then try and get a URL from Cloud Storage
          // else assume the source is a URL and proceed anyway.

          _store.UseOptions(new()
          {
            Endpoint = cloudCrate.Host,
            BucketName = cloudCrate.Bucket,
            Secure = true,
            AccessKey = "", // TODO fetch via oidc?
            SecretKey = "" // TODO fetch via oidc?
          });

          crateUrl = await _store.GetObjectUrl(cloudCrate.Path);
        }
      }
      catch (JsonException)
      {
        // assume the source is a URL and proceed anyway.
      }

      var crate = await _job.FetchRemoteRequestCrate(crateUrl);
      var acceptResult = _job.AcceptRequestCrate(job, crate);
      if (!acceptResult.IsSuccess)
      {
        await _status.ReportStatus(jobId, JobStatus.Failure,
          $"The remote Request Crate was not accepted: ${JsonSerializer.Serialize(acceptResult.Errors)}. Please resubmit the job.");

        await _job.Cleanup(job);
        return;
      }

      // Execute
      await _executeHandler.HandleAction(jobId);
    }
    catch (KeyNotFoundException e)
    {
      throw new InvalidOperationException(
        $"Hutch somehow queued an Action for a Job (id: {jobId}) that doesn't exist.", e);
    }
    catch
    {
      await _status.ReportStatus(jobId, JobStatus.Failure,
        $"An unrecoverable error occurred attempting to fetch the remote Request Crate. Please resubmit the Job.");
      await _job.Cleanup(jobId);
      throw;
    }
  }
}
