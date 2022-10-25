using Microsoft.FeatureManagement;

namespace HutchManager.Services;

public class RquestDistributionPollingService
{
  private readonly RQuestTaskApiClient _taskApi;
  private readonly ILogger<RquestDistributionPollingService> _logger;
  private readonly IFeatureManager _featureManager;
  private readonly JobQueueService _jobQueue;

  public RquestDistributionPollingService(
    RQuestTaskApiClient taskApi,
    ILogger<RquestDistributionPollingService> logger,
    IFeatureManager featureManager,
    JobQueueService jobQueue)
  {
    _taskApi = taskApi;
    _logger = logger;
    _featureManager = featureManager;
    _jobQueue = jobQueue;
  }
}
