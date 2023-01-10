using System.Text.Json;
using HutchManager.Constants;
using HutchManager.Data.Entities;
using HutchManager.Dto;
using Microsoft.FeatureManagement;

namespace HutchManager.Services;

public class AvailabilityPollingService
{
  private readonly RQuestTaskApiClient _taskApi;
  private readonly ILogger<AvailabilityPollingService> _logger;
  private readonly IFeatureManager _featureManager;
  private readonly IJobQueueService _jobQueue;

  public AvailabilityPollingService(
    RQuestTaskApiClient taskApi,
    ILogger<AvailabilityPollingService> logger,
    IFeatureManager featureManager,
    IJobQueueService jobQueue)
  {
    _logger = logger;
    _taskApi = taskApi;
    _featureManager = featureManager;
    _jobQueue = jobQueue;
  }

  public async Task Poll(ActivitySource activitySource)
  {
    AvailabilityQuery? job = null;

    do
    {
      try
      {
        job = await _taskApi.FetchQuery<AvailabilityQuery>(activitySource);
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

  public void SendToQueue(AvailabilityQuery jobPayload, string queueName)
  {
    // TODO: package jobPayload into an ActivityJob and send that to the queue
  }
}
