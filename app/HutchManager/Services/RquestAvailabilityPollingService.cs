using System.Text.Json;
using HutchManager.Constants;
using HutchManager.Data.Entities;
using HutchManager.Dto;
using Microsoft.FeatureManagement;

namespace HutchManager.Services;

public class RquestAvailabilityPollingService
{
  private readonly RQuestTaskApiClient _taskApi;
  private readonly ILogger<RquestAvailabilityPollingService> _logger;
  private readonly IFeatureManager _featureManager;
  private readonly IJobQueueService _jobQueue;

  public RquestAvailabilityPollingService(
    RQuestTaskApiClient taskApi,
    ILogger<RquestAvailabilityPollingService> logger,
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

        var packagedJob = PackageJob(job, activitySource);
        SendToQueue(packagedJob, activitySource.TargetDataSource.Id);
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

  private ActivityJob PackageJob(AvailabilityQuery jobPayload, ActivitySource activitySource)
  {
    var job = new ActivityJob
    {
      ActivitySourceId = activitySource.Id,
      Payload = JsonSerializer.SerializeToElement(jobPayload),
      Type = ActivityJobTypes.AvailabilityQuery,
      JobId = jobPayload.Uuid
    };
    return job;
  }

  private void SendToQueue(ActivityJob jobPayload, string queueName)
  {
    _jobQueue.SendMessage(queueName, jobPayload);
    _logger.LogInformation("Sent to Queue {Body}", JsonSerializer.Serialize(jobPayload));
  }
}
