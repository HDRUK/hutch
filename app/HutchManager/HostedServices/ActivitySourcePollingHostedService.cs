using HutchManager.Services;


namespace HutchManager.HostedServices
{
    public class ActivitySourcePollingHostedService : BackgroundService
    {
        private readonly ILogger<ActivitySourcePollingHostedService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private Timer? _timer;
      
        public ActivitySourcePollingHostedService(
            ILogger<ActivitySourcePollingHostedService> logger,
            IServiceProvider serviceProvider,
            IOptions<)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
          _logger.LogInformation("Activity Source Polling started");
          _timer = new Timer(TriggerActivitySourcePolling);
          RunTimerOnce();
          
          await PollRquest(stoppingToken);
        }

        public async void TriggerActivitySourcePolling(object? state)
        {
          // async void here is intentional to meet the TimerCallback signature
          // Stephen Cleary says it's ok:
          // https://stackoverflow.com/a/38918443
          
          
        }
        
        private void RunTimerOnce()
          => _timer?.Change(
            TimeSpan.FromSeconds(_config.QueryPollingInterval),
            Timeout.InfiniteTimeSpan);
        
        public async Task TriggerActivitySourcePolling(CancellationToken stoppingToken)
        {

          using (var scope = _serviceProvider.CreateScope())
          {
            var scopedProcessingService = 
              scope.ServiceProvider
                .GetRequiredService<IRquestPollingService>();

            await scopedProcessingService.PollRquest(stoppingToken);
          }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("RQUEST Polling Service stopping.");
            
            await base.StopAsync(stoppingToken);
        }

    }
}
