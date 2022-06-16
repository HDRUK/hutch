using LinkLiteManager.Services;


namespace LinkLiteManager.HostedServices
{
    public class RquestPollingHostedService : IHostedService
    {
        private readonly ILogger<RquestPollingHostedService> _logger;
        private readonly IServiceProvider _serviceProvider;
      
        public RquestPollingHostedService(
            ILogger<RquestPollingHostedService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        
        public async Task StartAsync(CancellationToken stoppingToken)
        {
          _logger.LogInformation("RQUEST Polling Service started.");
          await PollRquest(stoppingToken);
        }
        
        public async Task PollRquest(CancellationToken stoppingToken)
        {
          _logger.LogInformation(
            "Consume Scoped Service Hosted Service is working.");

          using (var scope = _serviceProvider.CreateScope())
          {
            var scopedProcessingService = 
              scope.ServiceProvider
                .GetRequiredService<IRquestPollingService>();

            await scopedProcessingService.PollRquest(stoppingToken);
          }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("RQUEST Polling Service stopping.");
            
            return Task.CompletedTask;
        }

    }
}
