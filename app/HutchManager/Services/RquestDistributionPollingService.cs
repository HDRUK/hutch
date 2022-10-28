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
  
  public async Task Poll(ActivitySource activitySource)
  {
    RquestDistributionQueryTask? job = null;

    do
    {
      try
      {
        job = await _taskApi.FetchQuery<RquestDistributionQueryTask>(activitySource);
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
  
  public void SendToQueue(RquestDistributionQueryTask jobPayload, string queueName)
  {
    ROCratesQuery roCratesQuery = new QueryTranslator.RquestDistributionQueryTranslator().Translate(jobPayload);
    _jobQueue.SendMessage(queueName, roCratesQuery);
    _logger.LogInformation("Sent to Queue {Body}", JsonSerializer.Serialize(roCratesQuery));
  }
}
