using System.Text;
using LinkLiteManager.Config;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace LinkLiteManager.Services;

public class QueryQueueService
{
  private readonly IConnection _connection;
  private readonly IModel _channel;
  private readonly IOptions<QueryQueueOptions> _options; 


  public QueryQueueService(IOptions<QueryQueueOptions> options)
  {
    _options = options;
    var connectionFactory = new ConnectionFactory()
    {
      HostName = options.Value.HostName,
      Port = options.Value.Port,
      UserName = options.Value.UserName,
      Password = options.Value.Password,
      VirtualHost = options.Value.VirtualHost
    };
    _connection = connectionFactory.CreateConnection();
    _channel = _connection.CreateModel();
    _channel.QueueDeclare(queue: options.Value.QueueName, durable: true);
  }

  public void SendMessage(string message)
  {
    var body = Encoding.UTF8.GetBytes(message);
    _channel.BasicPublish(
      exchange: null,
      routingKey: _options.Value.QueueName,
      body: body);
  }

  ~QueryQueueService()
  {
    _channel.Close();
    _connection.Close();
  }
}
