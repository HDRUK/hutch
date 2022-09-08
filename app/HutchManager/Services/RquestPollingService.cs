using System.Text.Json;
using HutchManager.Constants;
using HutchManager.Data.Entities;
using HutchManager.Dto;
using Microsoft.FeatureManagement;

namespace HutchManager.Services;

public class RQuestPollingService
{
  private readonly RQuestTaskApiClient _taskApi;
  private readonly ILogger<RQuestPollingService> _logger;
  private readonly IFeatureManager _featureManager;
  private readonly JobQueueService _jobQueue;

  public RQuestPollingService(
    RQuestTaskApiClient taskApi,
    ILogger<RQuestPollingService> logger,
    IFeatureManager featureManager,
    JobQueueService jobQueue)
  {
    _logger = logger;
    _taskApi = taskApi;
    _featureManager = featureManager;
    _jobQueue = jobQueue;
  }

  public async Task Poll(ActivitySource activitySource)
  {
    RquestQueryTask? job = null;

    do
    {
      try
      {
        job = await _taskApi.FetchQuery(activitySource);
        if (job is null)
        {
          _logger.LogInformation(
            "No Queries on Collection: {ResourceId}",
            activitySource.ResourceId);
          return;
        }

        await SendToQueue(job, activitySource.TargetDataSource.Id);
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

  public async Task SendToQueue(RquestQueryTask jobPayload, string queueName)
  {
    if (await _featureManager.IsEnabledAsync(Enum.GetName(FeatureFlags.UseROCrates)))
    {
      ROCratesQuery roCratesQuery = new QueryTranslator.RquestQueryTranslator().Translate(jobPayload);
      _jobQueue.SendMessage(queueName, roCratesQuery);
      _logger.LogInformation("Sent to Queue {Body}", JsonSerializer.Serialize(roCratesQuery));
    }
    else
    {
      _jobQueue.SendMessage(queueName, jobPayload);
      _logger.LogInformation("Sent to Queue {Body}", JsonSerializer.Serialize(jobPayload));
    }

  }
}
