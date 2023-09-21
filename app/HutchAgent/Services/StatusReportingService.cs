using HutchAgent.Constants;

namespace HutchAgent.Services;

/// <summary>
/// This is a first dummy implementation of Status Reporting for Hutch to report statuses to the TRE modules.
/// </summary>
public class StatusReportingService
{
  private readonly ILogger<StatusReportingService> _logger;

  public StatusReportingService(ILogger<StatusReportingService> logger)
  {
    _logger = logger;
  }

  public void ReportStatus(string jobId, JobStatus type, string? message = null)
  {
    // This is the most dummy version of this there could be <3
    _logger.LogInformation(
      "Job [{Id}] Status Report [{Type}]: {Message}",
      jobId, type.ToString(), message);
  }
}
