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
  private readonly MinioStoreServiceFactory _storeFactory;
  private readonly IFeatureManager _features;
  private readonly BagItService _bagIt;

  public FetchAndExecuteActionHandler(
    ExecuteActionHandler executeHandler,
    WorkflowJobService jobs,
    StatusReportingService status,
    JobLifecycleService job,
    MinioStoreServiceFactory storeFactory,
    IFeatureManager features, BagItService bagIt)
  {
    _executeHandler = executeHandler;
    _jobs = jobs;
    _status = status;
    _job = job;
    _storeFactory = storeFactory;
    _features = features;
    _bagIt = bagIt;
  }

  public async Task HandleAction(string jobId, object? payload)
  {
    try
    {
      var job = await _jobs.Get(jobId);

      if (string.IsNullOrWhiteSpace(job.CrateSource))
      {
        await _status.ReportStatus(jobId, JobStatus.Failure,
          $"The remote crate could not be fetched from the provided source: {job.CrateSource}");

        if (!await _features.IsEnabledAsync(FeatureFlags.RetainFailures))
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
          var store = await _storeFactory.Create(new()
          {
            Host = cloudCrate.Host,
            Bucket = cloudCrate.Bucket,
            AccessKey = cloudCrate.AccessKey,
            SecretKey = cloudCrate.SecretKey,
            Secure = cloudCrate.Secure
          });

          crateUrl = await store.GetObjectUrl(cloudCrate.Path);
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

        if (!await _features.IsEnabledAsync(FeatureFlags.RetainFailures))
          await _job.Cleanup(job);
        return;
      }

      // throw invalid data if checksums don't match
      if (!await _bagIt.VerifyChecksums(jobId.BagItPayloadPath())) throw new InvalidDataException();

      // Execute
      await _executeHandler.HandleAction(jobId, null);
    }
    catch (KeyNotFoundException e)
    {
      throw new InvalidOperationException(
        $"Hutch somehow queued an Action for a Job (id: {jobId}) that doesn't exist.", e);
    }
    catch (InvalidDataException)
    {
      await _status.ReportStatus(jobId, JobStatus.Failure,
        "The files' checksums do not match. Check their contents, remake the checksums and re-submit the job.");

      if (!await _features.IsEnabledAsync(FeatureFlags.RetainFailures))
        await _job.Cleanup(jobId);

      throw;
    }
    catch
    {
      await _status.ReportStatus(jobId, JobStatus.Failure,
        $"An unrecoverable error occurred attempting to fetch the remote Request Crate. Please resubmit the Job.");

      if (!await _features.IsEnabledAsync(FeatureFlags.RetainFailures))
        await _job.Cleanup(jobId);

      throw;
    }
  }
}
