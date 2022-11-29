namespace HutchManager.Services;

public interface IJobQueueService
{
  void SendMessage<T>(string queueName, T message);
}
