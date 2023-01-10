using System.Text.Json;
using HutchManager.Data.Entities;
using HutchManager.Dto;
using Microsoft.FeatureManagement;

namespace HutchManager.Services;

public class RquestDistributionPollingService
{
  private readonly RQuestTaskApiClient _taskApi;
  private readonly ILogger<RquestDistributionPollingService> _logger;
  private readonly IFeatureManager _featureManager;
  private readonly IJobQueueService _jobQueue;

  public RquestDistributionPollingService(
    RQuestTaskApiClient taskApi,
    ILogger<RquestDistributionPollingService> logger,
    IFeatureManager featureManager,
    IJobQueueService jobQueue)
  {
    _taskApi = taskApi;
    _logger = logger;
    _featureManager = featureManager;
    _jobQueue = jobQueue;
  }
  
  public async Task Poll(ActivitySource activitySource)
  {
    DistributionQuery? job = null;

    do
    {
      try
      {
        job = await _taskApi.FetchQuery<DistributionQuery>(activitySource);
        if (job is null)
        {
          _logger.LogInformation(
            "No Queries on Collection: {ResourceId}",
            activitySource.ResourceId);
          return;
        }

        SendToQueue(job, activitySource.TargetDataSource.Id);
      }
      catch (Exception e)
      {
        if (job is null)
        {
          _logger.LogError(e, "Task fetching failed");
        }
        else
        {
          throw;
        }
      }
    } while (job is null);
  }
  
  public void SendToQueue(DistributionQuery jobPayload, string queueName)
  {
    // TODO: package jobPayload into an ActivityJob and send that to the queue
  }
}
