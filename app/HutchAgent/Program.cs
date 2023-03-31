using HutchAgent.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HutchAgent;
public class Program
  {
    public static void Main(string[] args)
    {
      var builder = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false);
      IConfiguration config = builder.Build();
      new WorkflowTriggerService(config.GetSection("WfexsOptions")).TriggerWfexs();
    }
  }
