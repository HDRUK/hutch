using HutchAgent.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HutchAgent;
public class Program
  {
    public static void Main(string[] args)
    {
      var serviceProvider = new ServiceCollection()
        .AddLogging()
        .AddTransient<WorkflowTriggerService>()
        .BuildServiceProvider();
      new WorkflowTriggerService().TriggerWfexs();
    }
  }
