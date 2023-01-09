using System.Text.Json;
using Azure.Storage.Queues;
using HutchManager.Config;
using Microsoft.Extensions.Options;

namespace HutchManager.Services;

public class AzureJobQueueService : IJobQueueService
{
  private readonly string _connectionString;

  public AzureJobQueueService(IOptions<AzureJobQueueOptions> options)
  {
    _connectionString = options.Value.ConnectionString;
  }

  public void SendMessage<T>(string queueName, T message) where T : class
  {
    var queueClient = new QueueClient(_connectionString, queueName);
    queueClient.CreateIfNotExists();
    queueClient.SendMessage(
      JsonSerializer.Serialize(message));
  }
}
