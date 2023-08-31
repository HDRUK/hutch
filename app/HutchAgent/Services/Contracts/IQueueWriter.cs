namespace HutchAgent.Services;

public interface IQueueWriter
{
  void SendMessage<T>(string queueName, T message) where T : class;
}
