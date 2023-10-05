using HutchAgent.Constants;
using Microsoft.FeatureManagement;

namespace HutchAgent.Services;

/// <summary>
/// This is a first dummy implementation of Status Reporting for Hutch to report statuses to the TRE modules.
/// </summary>
public class StatusReportingService
{
  private readonly ILogger<StatusReportingService> _logger;
  private readonly ControllerApiService _controllerApi;
  private readonly IFeatureManager _features;

  public StatusReportingService(ILogger<StatusReportingService> logger, ControllerApiService controllerApi,
    IFeatureManager features)
  {
    _logger = logger;
    _controllerApi = controllerApi;
    _features = features;
  }

  /// <summary>
  /// Report status to the submission layer.
  /// Only log the status change in Standalone Mode.
  /// </summary>
  /// <param name="jobId"></param>
  /// <param name="type"></param>
  /// <param name="message"></param>
  public async Task ReportStatus(string jobId, JobStatus type, string? message = null)
  {
    // This is the most dummy version of this there could be <3
    _logger.LogInformation(
      "Job [{Id}] Status Report [{Type}]: {Message}",
      jobId, type.ToString(), message);

    try
    {
      if (await _features.IsEnabledAsync(FeatureFlags.StandaloneMode))
        return;

      await _controllerApi.UpdateStatusForTre(jobId, type, message);
    }
    catch (Exception e)
    {
      _logger.LogError(exception: e, "Unable to report status to submission layer");
      throw;
    }
  }
}
