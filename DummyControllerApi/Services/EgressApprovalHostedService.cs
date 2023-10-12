using System.Text.Json;
using DummyControllerApi.Models;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;

namespace DummyControllerApi.Services;

public class EgressApprovalHostedService : BackgroundService
{
  private readonly ILogger<EgressApprovalHostedService> _logger;
  private readonly IServiceProvider _serviceProvider;

  public EgressApprovalHostedService(
    ILogger<EgressApprovalHostedService> logger,
    IServiceProvider serviceProvider)
  {
    _logger = logger;
    _serviceProvider = serviceProvider;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    while (!stoppingToken.IsCancellationRequested)
    {
      try
      {
        // Every 5 seconds (unless we're busy) we poll the in memory queue for unprocessed approvals
        var delay = Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        using var scope = _serviceProvider.CreateScope();

        var queue = _serviceProvider.GetRequiredService<InMemoryApprovalQueue>();
        var config = _serviceProvider.GetRequiredService<IConfiguration>();

        if (queue.HasItems())
        {
          var subId = queue.Dequeue();
          
          // delay 5 seconds after picking it up to ensure Hutch is ready and to "pretend" to approve stuff
          await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
          
          // TODO in future the egress request should be able to tell Hutch none is required...

          // TODO make an HTTP Request to Hutch
          var url = Url.Combine(config["HutchApiBaseUrl"], "jobs", subId, "approval");

          await url.PostJsonAsync(new HutchApprovalRequestModel
          {
            Files = new() { ["file1"] = true, ["file2"] = true }
          }, cancellationToken: stoppingToken);
        }

        await delay;
      }
      catch (Exception e) // exceptions shouldn't bring down the whole HostedService
      {
        _logger.LogError(e,
          "EgressApproval threw an Exception while running");
      }
    }
  }

  public override async Task StopAsync(CancellationToken stoppingToken)
  {
    _logger.LogInformation("Stopping polling WorkflowJob Action Queue");

    await base.StopAsync(stoppingToken);
  }
}
