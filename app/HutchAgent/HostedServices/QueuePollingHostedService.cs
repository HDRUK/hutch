using HutchAgent.Config;
using HutchAgent.Constants;
using HutchAgent.Services;
using Microsoft.Extensions.Options;

namespace HutchAgent.HostedServices;

public class QueuePollingHostedService : BackgroundService
{
  private readonly JobActionsQueueOptions _options;
  private readonly ILogger<QueuePollingHostedService> _logger;
  private readonly IServiceProvider _serviceProvider;

  private List<Task> _runningActions = new();

  public QueuePollingHostedService(
    IOptions<JobActionsQueueOptions> options,
    ILogger<QueuePollingHostedService> logger,
    IServiceProvider serviceProvider)
  {
    _options = options.Value;
    _logger = logger;
    _serviceProvider = serviceProvider;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    _logger.LogInformation("Polling WorkflowJob Action Queue...");


    while (!stoppingToken.IsCancellationRequested)
    {
      var delay = Task.Delay(TimeSpan.FromSeconds(_options.PollingIntervalSeconds), stoppingToken);


      using (var scope = _serviceProvider.CreateScope())
      {
        var queue = _serviceProvider.GetRequiredService<IQueueReader>();
        var workflowTriggerService = scope.ServiceProvider.GetService<WorkflowTriggerService>() ?? throw new InvalidOperationException();

        // If a thread is available, per Max Parallelism, then
        // Pop a queue message, and Execute its action on the free thread
        if (_runningActions.Count < _options.MaxParallelism)
        {
          var message = queue.Pop(_options.QueueName);
          if (message is null)
          {
            await delay;
            continue;
          }

          // Optionally, prepare the job's crate and state record before executing?

          switch (message.ActionType)
          {
            case JobActionTypes.Execute:
              _runningActions.Add(Task.Run(async () => await workflowTriggerService.TriggerWfexs(message.JobId),
                stoppingToken));
              break;
            case JobActionTypes.Finalize:
              _runningActions.Add(Task.Run(async () => _logger.LogInformation("Finalizing..."), stoppingToken));
              break;
          }
        }
      }

      // Stop tracking completed actions
      _runningActions = _runningActions.Where(x => !x.IsCompleted).ToList();

      await delay;
    }
  }

  public override async Task StopAsync(CancellationToken stoppingToken)
  {
    _logger.LogInformation("Stopping polling WorkflowJob Action Queue.");

    await base.StopAsync(stoppingToken);
  }
}
