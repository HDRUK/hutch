using System.Text;
using HutchManager.Config;
using HutchManager.Data;
using HutchManager.Dto;
using HutchManager.HostedServices;
using HutchManager.OptionsModels;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Microsoft.FeatureManagement;
namespace HutchManager.Services;

internal interface IRquestPollingService
{
  Task PollRquest(CancellationToken stoppingToken);
}
public class RquestPollingService : IRquestPollingService
{
  private readonly RquestTaskApiClient _rquestApi;
  private readonly ILogger<RquestPollingService> _logger;
  private readonly RquestPollingServiceOptions _config;
  private readonly ApplicationDbContext _db;
  private readonly IFeatureManager _featureManager;
  private Timer? _timer;
  private int executionCount = 0;

  public RquestPollingService(
    RquestTaskApiClient rquestApi,
    ILogger<RquestPollingService> logger,
    IOptions<RquestPollingServiceOptions> config,
    ApplicationDbContext db,
    IFeatureManager featureManager)
  {
    _logger = logger;
    _rquestApi = rquestApi;
    _config = config.Value;
    _db = db;
    _featureManager = featureManager;
  }
  public async Task PollRquest(CancellationToken stoppingToken)
  {
    while (!stoppingToken.IsCancellationRequested)
    {
      executionCount++;

      _logger.LogDebug(
        "{Service} is working.Count: {Count}", nameof(RquestPollingService), executionCount);
      _timer = new Timer(PollRquest);
      RunTimerOnce();
      await Task.Delay(10000, stoppingToken);
    }
  }
  private async void PollRquest(object? state)
  {
    // async void here is intentional to meet the TimerCallback signature
    // Stephen Cleary says it's ok:
    // https://stackoverflow.com/a/38918443
    var count = Interlocked.Increment(ref executionCount);
    var activitySource = _db.ActivitySources.First();
    _logger.LogInformation(
      "Timed {HostedService} is working. Count: {Count}", nameof(RquestPollingHostedService), count);
    _logger.LogInformation(
    "Polling RQUEST for Queries on Collection: {_resourceId}",
   activitySource.ResourceId);

    RquestQueryTask? job = null;
    int? result = null;

    try
    {
      job = await _rquestApi.FetchQuery(activitySource);
      if (job is null)
      {
        _logger.LogInformation(
              "No Queries on Collection: {_resourceId}",
              activitySource.ResourceId);
        RunTimerOnce();
        return;
      }

      _logger.LogInformation("Query {task}", job.Query);
      if (await _featureManager.IsEnabledAsync(FeatureFlags.ROCratesTranslation))
      {
        ROCratesQuery roCratesQuery = new QueryTranslator.RquestQueryTranslator().Translate(job);
        
      }
      
      SendToQueue(job);
      // TODO: Threading / Parallel query handling?
      // affects timer usage, the process logic will need to be
      // threaded using Task.Run or similar.
      StopTimer();
    }
    catch (Exception e)
    {
      if (job is null)
      {
        _logger.LogError(e, "Task fetching failed");
      }
      else
      {

        if (result is null)
        {
          _logger.LogError(e,
              "Query execution failed for job: {jobId}",
              job.JobId);
        }
        else
        {
          _logger.LogError(e,
              "Results Submission failed for job: {jobId}, result: {result}",
              job.JobId,
              result);
        }

        _logger.LogInformation("Cancelled failed job: {jobId}", job.JobId);
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

  public void SendToQueue(RquestQueryTask jobPayload)
  {
    var factory = new ConnectionFactory() { HostName = "localhost" };
    using (var connection = factory.CreateConnection())
    using (var channel = connection.CreateModel())
    {
      channel.QueueDeclare(queue: "jobs",
        durable: true,
        exclusive: false,
        autoDelete: false,
        arguments: null);

      var properties = channel.CreateBasicProperties();
      properties.Persistent = true;

      byte[] body = Encoding.Default.GetBytes(JsonConvert.SerializeObject(jobPayload));
      channel.BasicPublish(exchange: "",
        routingKey: "jobs",
        basicProperties: null,
        body: body);

      _logger.LogInformation("Sent to Queue {body}", jobPayload);
    }
  }

}
