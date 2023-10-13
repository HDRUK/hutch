using System.Text.Json;
using HutchAgent.Config;
using HutchAgent.Constants;
using HutchAgent.Models.JobQueue;
using HutchAgent.Services.ActionHandlers;
using HutchAgent.Services.Contracts;
using Microsoft.Extensions.Options;
using RabbitMQ.Client.Exceptions;

namespace HutchAgent.Services.Hosted;

public class JobActionQueuePoller : BackgroundService
{
  private readonly JobActionsQueueOptions _options;
  private readonly ILogger<JobActionQueuePoller> _logger;
  private readonly IServiceProvider _serviceProvider;

  private List<Task> _runningActions = new();

  public JobActionQueuePoller(
    IOptions<JobActionsQueueOptions> options,
    ILogger<JobActionQueuePoller> logger,
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
        try
        {
          var queue = _serviceProvider.GetRequiredService<IQueueReader>();


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

            // Define ActionHandlers and optionally PayloadModel for each type
            var handlers = new Dictionary<string, (Type handlerType, Type? payloadType)>
            {
              [JobActionTypes.FetchAndExecute] = (typeof(FetchAndExecuteActionHandler), null),
              [JobActionTypes.Execute] = (typeof(ExecuteActionHandler), null),
              [JobActionTypes.InitiateEgress] =
                (typeof(InitiateEgressActionHandler), typeof(InitiateEgressPayloadModel)),
              [JobActionTypes.Finalize] = (typeof(FinalizeActionHandler), null)
            };

            // Get the Handler and Handle its Action
            if (!handlers.ContainsKey(message.ActionType))
            {
              _logger.LogError("Encountered unknown Action Type in queue. QueueMessage: {Message}",
                JsonSerializer.Serialize(message));
              await delay;
              continue;
            }

            var handler = (IActionHandler)scope.ServiceProvider
              .GetRequiredService(handlers[message.ActionType].handlerType);

            try
            {
              // deserialize payload if present and a payload type is configured for the handler
              var payloadType = handlers[message.ActionType].payloadType;
              var payload = payloadType is not null && message.Payload is not null
                ? JsonSerializer.Deserialize(message.Payload, payloadType)
                : null;
              
              await handler.HandleAction(message.JobId, payload);
            }
            catch (Exception e) // ActionHandler exceptions shouldn't bring down the HostedService
            {
              _logger.LogError(e,
                "{ActionType}ActionHandler threw an Exception while running for Job: {JobId}",
                message.ActionType,
                message.JobId);
            }
          }
        }
        catch (BrokerUnreachableException e)
        {
          _logger.LogCritical(e, "Couldn't connect to RabbitMQ. Is it running and are the connection details correct?");
          _logger.LogCritical(
            "Background jobs cannot run without RabbitMQ, and therefore Hutch will not function correctly!");
          break;
        }
      }

      // Stop tracking completed actions
      _runningActions = _runningActions.Where(x => !x.IsCompleted).ToList();

      await delay;
    }
  }

  public override async Task StopAsync(CancellationToken stoppingToken)
  {
    _logger.LogInformation("Stopping polling WorkflowJob Action Queue");

    await base.StopAsync(stoppingToken);
  }
}
