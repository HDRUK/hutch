using HutchAgent.Config;
using Microsoft.Extensions.Options;

namespace HutchAgent.Services;

public class WatchFolderService : BackgroundService
{
  private readonly WatchFolderOptions _options;
  private readonly ILogger<WatchFolderService> _logger;
  private readonly MinioService _minioService;

  public WatchFolderService(IOptions<WatchFolderOptions> options, ILogger<WatchFolderService> logger,
    MinioService minioService)
  {
    _options = options.Value;
    _logger = logger;
    _minioService = minioService;
  }

  /// <summary>
  /// Watch a folder for results of WfExS runs.
  /// </summary>
  /// <param name="stoppingToken"></param>
  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    _logger.LogInformation($"Starting to watch folder {_options.Path}");

    while (!stoppingToken.IsCancellationRequested)
    {
      _watchFolder(_options.Path);

      await Task.Delay(TimeSpan.FromSeconds(_options.PollingIntervalSeconds), stoppingToken);
    }
  }

  /// <summary>
  /// Stop watching the results folder for WfExS execution results.
  /// </summary>
  /// <param name="stoppingToken"></param>
  public override async Task StopAsync(CancellationToken stoppingToken)
  {
    _logger.LogInformation($"Stopping watching folder {_options.Path}");

    await base.StopAsync(stoppingToken);
  }

  private async void _watchFolder(string folderName)
  {
  }
}
