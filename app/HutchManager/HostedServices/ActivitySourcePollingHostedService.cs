using HutchManager.Constants;
using HutchManager.Data;
using HutchManager.OptionsModels;
using HutchManager.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


namespace HutchManager.HostedServices
{
    public class ActivitySourcePollingHostedService : BackgroundService
    {
        private readonly ILogger<ActivitySourcePollingHostedService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ActivitySourcePollingOptions _config;
        private int _executionCount;

        public ActivitySourcePollingHostedService(
            ILogger<ActivitySourcePollingHostedService> logger,
            IServiceProvider serviceProvider,
            IOptions<ActivitySourcePollingOptions> config)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _config = config.Value;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
          _logger.LogInformation("Activity Source Polling started");

          if (_config.PollingInterval < 0) return;
          
          while (!stoppingToken.IsCancellationRequested)
          {
            var executionTask = TriggerActivitySourcePolling();
            
            await Task.Delay(TimeSpan.FromSeconds(_config.PollingInterval), stoppingToken);
          }
        }

        public async Task TriggerActivitySourcePolling()
        {
          var count = Interlocked.Increment(ref _executionCount);
            
          _logger.LogDebug(
            "{Service} is working. Count: {Count}",nameof(RQuestPollingService) , count);

          using var executionScope = _serviceProvider.CreateScope();

          // TODO use another worker service with DI instead of Service Locator? 
          var db = executionScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
          
          var rQuestPoller = executionScope.ServiceProvider.GetRequiredService<RQuestPollingService>();

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
            _logger.LogInformation("Activity Source Polling stopping");
            
            await base.StopAsync(stoppingToken);
        }

    }
}
