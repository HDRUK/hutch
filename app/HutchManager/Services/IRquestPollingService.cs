using HutchManager.Data.Entities;
using HutchManager.Dto;

namespace HutchManager.Services;

public interface IRquestPollingService<T>
{
  Task Poll(ActivitySource activitySource);
  void SendToQueue(T jobPayload, string queueName);
}
