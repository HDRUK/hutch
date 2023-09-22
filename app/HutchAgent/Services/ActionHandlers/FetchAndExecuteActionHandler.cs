using System.Text.Json;
using HutchAgent.Constants;
using HutchAgent.Data;
using HutchAgent.Services.Contracts;

namespace HutchAgent.Services.ActionHandlers;

public class FetchAndExecuteActionHandler : IActionHandler
{
  private readonly ExecuteActionHandler _executeHandler;
  private readonly JobLifecycleService _job;
  private readonly WorkflowJobService _jobs;
  private readonly StatusReportingService _status;

  public FetchAndExecuteActionHandler(
    ExecuteActionHandler executeHandler,
    WorkflowJobService jobs,
    StatusReportingService status,
    JobLifecycleService job)
  {
    _executeHandler = executeHandler;
    _jobs = jobs;
    _status = status;
    _job = job;
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
      var crate = await _job.FetchRemoteRequestCrate(job.CrateSource);
      var accepted = _job.AcceptRequestCrate(job, crate);
      if (!accepted.Success)
      {
        await _status.ReportStatus(jobId, JobStatus.Failure,
          $"The remote Request Crate was not accepted: ${JsonSerializer.Serialize(accepted.Errors)}. Please resubmit the job.");

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
