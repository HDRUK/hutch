using HutchAgent.Constants;

namespace HutchAgent.Services;

/// <summary>
/// This is a first dummy implementation of Status Reporting for Hutch to report statuses to the TRE modules.
/// </summary>
public class StatusReportingService
{
  private readonly ILogger<StatusReportingService> _logger;
  private readonly ControllerApiService _controllerApi;

  public StatusReportingService(ILogger<StatusReportingService> logger, ControllerApiService controllerApi)
  {
    _logger = logger;
    _controllerApi = controllerApi;
  }

  public Task ReportStatus(string jobId, JobStatus type, string? message = null)
  {
    // This is the most dummy version of this there could be <3
    _logger.LogInformation(
      "Job [{Id}] Status Report [{Type}]: {Message}",
      jobId, type.ToString(), message);

    return Task.CompletedTask;
  }
}
