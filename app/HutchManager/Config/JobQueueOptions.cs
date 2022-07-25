using RabbitMQ.Client;

namespace HutchManager.Config;

public class JobQueueOptions
{
  public string HostName { get; set; } = "";
  public int Port { get; set; } = AmqpTcpEndpoint.UseDefaultPort;
  public string UserName { get; set; } = ConnectionFactory.DefaultUser;
  public string Password { get; set; } = ConnectionFactory.DefaultPass;
}
