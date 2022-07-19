using System.Text;
using System.Text.Json;
using HutchManager.Config;
using HutchManager.Constants;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace HutchManager.Services;

public class JobQueueService : IDisposable
{
  private readonly IConnection _connection;
  private readonly IModel _channel;
  private readonly JobQueueOptions _options;


  public JobQueueService(IOptions<JobQueueOptions> options)
  {
    _options = options.Value;
    var connectionFactory = new ConnectionFactory()
    {
      HostName = _options.HostName,
      Port = _options.Port,
      UserName = _options.UserName,
      Password = _options.Password
    };
    _connection = connectionFactory.CreateConnection();
    _channel = _connection.CreateModel();
  }

  /// <summary>
  /// Serialize an object and send it, serialised to JSON, as a message to the named queue
  /// </summary>
  /// <param name="queueName"></param>
  /// <param name="message"></param>
  /// <typeparam name="T">The CLR type of the message</typeparam>
  public void SendMessage<T>(string queueName, T message)
  where T: class
  {
    _channel.QueueDeclare(queue: queueName,
      durable: false,
      exclusive: false,
      autoDelete: false,
      arguments: null);
    
    var body = Encoding.UTF8.GetBytes(
      JsonSerializer.Serialize(message, DefaultJsonOptions.Serializer));
    _channel.BasicPublish(
      exchange: null,
      basicProperties: null,
      routingKey: queueName,
      body: body);
  }

  ~JobQueueService()
  {
    Dispose(false);
  }

  private void ReleaseUnmanagedResources()
  {
    // TODO release unmanaged resources here
  }

  private void Dispose(bool disposing)
  {
    ReleaseUnmanagedResources();
    if (disposing)
    {
      _connection.Dispose();
      _channel.Dispose();
    }
  }

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }
}
