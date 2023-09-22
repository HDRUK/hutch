using System.Text;
using System.Text.Json;
using HutchAgent.Config;
using HutchAgent.Models;
using HutchAgent.Models.JobQueue;
using HutchAgent.Services.Contracts;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace HutchAgent.Services;

public class RabbitQueueReader : IQueueReader
{
  private readonly RabbitQueueOptions _queueOptions;
  private readonly IConnection _connection;
  private readonly IModel _channel;

  public RabbitQueueReader(IOptions<RabbitQueueOptions> queueOptions)
  {
    _queueOptions = queueOptions.Value;
    var connectionFactory = new ConnectionFactory()
    {
      HostName = _queueOptions.HostName,
      Port = _queueOptions.Port,
      UserName = _queueOptions.UserName,
      Password = _queueOptions.Password
    };
    _connection = connectionFactory.CreateConnection();
    _channel = _connection.CreateModel();
  }

  /// <inheritdoc/>
  public JobQueueMessage? Pop(string queueName)
  {
    _channel.QueueDeclare(queue: queueName,
      durable: false,
      exclusive: false,
      autoDelete: false,
      arguments: null);

    var result = _channel.BasicGet(queueName, autoAck: true);
    if(result is null) return null;

    var body = Encoding.UTF8.GetString(result.Body.ToArray());
    return JsonSerializer.Deserialize<JobQueueMessage>(body);
  }
}
