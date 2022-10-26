using HutchManager.Constants;
using HutchManager.Data;
using HutchManager.OptionsModels;
using HutchManager.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


namespace HutchManager.HostedServices;

public class DistributionPollingHostedService: BackgroundService
{
  private readonly ILogger<DistributionPollingHostedService> _logger;
  private readonly IServiceProvider _serviceProvider;
  private readonly ActivitySourcePollingOptions _config;
  private int _executionCount;

  public DistributionPollingHostedService(
    ILogger<DistributionPollingHostedService> logger,
    IServiceProvider serviceProvider,
    ActivitySourcePollingOptions config)
  {
    _logger = logger;
    _serviceProvider = serviceProvider;
    _config = config;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    _logger.LogInformation("Distribution Polling started");

    if (_config.PollingInterval < 0) return;
          
    while (!stoppingToken.IsCancellationRequested)
    {
      var executionTask = TriggerActivitySourcePolling();
            
      await Task.Delay(TimeSpan.FromDays(_config.PollingInterval), stoppingToken);
    }
  }
  
  public async Task TriggerActivitySourcePolling()
  {
    var count = Interlocked.Increment(ref _executionCount);
            
    _logger.LogDebug(
      "{Service} is working. Count: {Count}",nameof(RquestDistributionPollingService) , count);

    using var executionScope = _serviceProvider.CreateScope();

    // TODO use another worker service with DI instead of Service Locator? 
    var db = executionScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
          
    var rQuestPoller = executionScope.ServiceProvider.GetRequiredService<RquestDistributionPollingService>();

    var sources = db.ActivitySources.AsNoTracking()
      .Include(x => x.Type)
      .Include(x => x.TargetDataSource)
      .ToList();

    List<Task> pollTasks = new();

    foreach (var source in sources)
    {
      switch (source.Type.Id)
      {
        case SourceTypes.RQuest:
          pollTasks.Add(rQuestPoller.Poll(source));
          break;
        default:
          _logger.LogError("Unknown Activity Source Type cannot be handled: {SourceType}", source.Type.Id);
          break;
      }
    }

    await Task.WhenAll(pollTasks);
  }
        
  public override async Task StopAsync(CancellationToken stoppingToken)
  {
    _logger.LogInformation("Distribution Polling stopping");
            
    await base.StopAsync(stoppingToken);
  }
}
