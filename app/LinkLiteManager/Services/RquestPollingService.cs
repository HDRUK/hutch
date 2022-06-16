using LinkLiteManager.Dto;
using LinkLiteManager.OptionsModels;
using Microsoft.Extensions.Options;

namespace LinkLiteManager.Services;

internal interface IRquestPollingService
{
  Task PollRquest(CancellationToken stoppingToken);
}
public class RquestPollingService: IRquestPollingService
{
  private readonly RquestConnectorApiClient _rquestApi;
  private readonly ILogger<RquestPollingService> _logger;
  private readonly RquestPollingServiceOptions _config;
  private Timer? _timer;
  private int executionCount = 0;
  
  public RquestPollingService(
    RquestConnectorApiClient rquestApi,
    ILogger<RquestPollingService> logger,
    IOptions<RquestPollingServiceOptions> config)
  {
    _logger = logger;
    _rquestApi = rquestApi;
    _config = config.Value;
  }
  public async Task PollRquest(CancellationToken stoppingToken)
  {
    while (!stoppingToken.IsCancellationRequested)
    {
      executionCount++;

      _logger.LogInformation(
        "Scoped Processing Service is working. Count: {Count}", executionCount);
      _timer = new Timer(PollRquest);
      RunTimerOnce();
      await Task.Delay(10000, stoppingToken);
    }
  }
  private async void PollRquest(object? state)
        {
            var count = Interlocked.Increment(ref executionCount);
          _logger.LogInformation(
            "Timed Hosted Service is working. Count: {Count}", count);
          _logger.LogInformation(
            "Polling RQUEST for Queries on Collection: {_collectionId}",
           _config.RquestCollectionId);
            // async void here is intentional to meet the TimerCallback signature
            // Stephen Cleary says it's ok:
            // https://stackoverflow.com/a/38918443

            _logger.LogInformation(
                "Polling RQUEST for Queries on Collection: {_collectionId}",
                _config.RquestCollectionId);


            RquestQueryTask? task = null;
            int? result = null;

            try
            {
                task = await _rquestApi.FetchQuery(_config.RquestCollectionId);

                if (task is null)
                {
                    _logger.LogInformation(
                          "No Queries on Collection: {_collectionId}",
                          _config.RquestCollectionId);
                    RunTimerOnce();
                    return;
                }

                // TODO: Threading / Parallel query handling?
                // affects timer usage, the process logic will need to be
                // threaded using Task.Run or similar.
                StopTimer();

                _logger.LogInformation("Processing Query: {taskId}", task.TaskId);
              
                _logger.LogInformation(
                    "Query Result: {taskId}: {result}", task.TaskId, result);
            }
            catch (Exception e)
            {
                if (task is null)
                {
                    _logger.LogError(e, "Task fetching failed");
                }
                else
                {

                    if (result is null)
                    {
                        _logger.LogError(e,
                            "Query execution failed for task: {taskId}",
                            task.TaskId);
                    }
                    else
                    {
                        _logger.LogError(e,
                            "Results Submission failed for task: {taskId}, result: {result}",
                            task.TaskId,
                            result);
                    }

                    await _rquestApi.CancelQueryTask(task.TaskId);
                    _logger.LogInformation("Cancelled failed task: {taskId}", task.TaskId);
                }
            }

            RunTimerOnce();
        }
        private void RunTimerOnce()
          => _timer?.Change(
            TimeSpan.FromSeconds(_config.QueryPollingInterval),
            Timeout.InfiniteTimeSpan);

        /// <summary>
        /// Change the timer to execute its callback regularly
        /// at the configured polling interval
        /// </summary>
        private void StartTimer()
          => _timer?.Change(
            TimeSpan.Zero,
            TimeSpan.FromSeconds(_config.QueryPollingInterval));

        /// <summary>
        /// Change the timer to stop regular callback execution
        /// </summary>
        private void StopTimer()
          => _timer?.Change(Timeout.Infinite, 0);

}
