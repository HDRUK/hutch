using HutchAgent.Models;
using HutchAgent.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HutchAgent;
public class Program
  {
    public static void Main(string[] args)
    {

      IHost host = Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
          services.AddHostedService<WorkflowTriggerService>();
          services.AddOptions<WorkflowTriggerOptions>().Bind(hostContext.Configuration.GetSection("Wfexs"));
        })
        .Build();
      host.Run();
    }
  }
