using HutchAgent.Config;
using Microsoft.Extensions.Options;
using Minio.Exceptions;

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
  /// Watch a folder for results of WfExS runs and upload to an S3 bucket.
  /// </summary>
  /// <param name="stoppingToken"></param>
  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    _logger.LogInformation($"Starting to watch folder {_options.Path}");

    while (!stoppingToken.IsCancellationRequested)
    {
      _watchFolder();

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

  private async void _watchFolder()
  {
    foreach (var file in Directory.EnumerateFiles(_options.Path))
    {
      if (await _minioService.FileExistsInBucket(Path.GetFileName(file)))
      {
        _logger.LogInformation($"{Path.GetFileName(file)} already exists in S3.");
        continue;
      }

      try
      {
        _logger.LogInformation($"Attempting to upload {file} to S3.");
        await _minioService.UploadToBucket(file);
        _logger.LogInformation($"Successfully uploaded {file} to S3.");
      }
      catch (BucketNotFoundException e)
      {
        _logger.LogCritical($"Unable to upload {file} to S3. The configured bucket does not exist.");
      }
      catch (MinioException e)
      {
        _logger.LogError($"Unable to upload {file} to S3. An error occurred with the S3 server.");
      }
    }
  }
}
