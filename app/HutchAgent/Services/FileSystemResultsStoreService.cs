using HutchAgent.Config;
using Microsoft.Extensions.Options;

namespace HutchAgent.Services;

public class FileSystemResultsStoreService : IResultsStoreWriter
{
  private readonly FileSystemResultsStoreOptions _options;
  private readonly ILogger<FileSystemResultsStoreService> _logger;

  public FileSystemResultsStoreService(IOptions<FileSystemResultsStoreOptions> options,
    ILogger<FileSystemResultsStoreService> logger)
  {
    _options = options.Value;
    _logger = logger;
  }

  /// <summary>
  /// Check if the location of the store folder exists on disk.
  /// </summary>
  /// <returns><c>true</c>if the store location exists, else <c>false</c>.</returns>
  public bool StoreExists()
  {
    return Directory.Exists(_options.Path);
  }

  /// <summary>
  /// Write the result file to results store location.
  /// </summary>
  /// <param name="resultPath">The path to the result file to be written to the store.</param>
  /// <exception cref="DirectoryNotFoundException">The results store does not exists.</exception>
  /// <exception cref="FileNotFoundException">The file to write to the store does not exist.</exception>
  public void WriteToStore(string resultPath)
  {
    if (!StoreExists()) throw new DirectoryNotFoundException($"No such bucket: {_options.Path}");

    if (!File.Exists(resultPath)) throw new FileNotFoundException();

    _logger.LogInformation($"Saving {resultPath} to {_options.Path}...");
    File.Copy(
      resultPath,
      Path.Combine(_options.Path, Path.GetFileName(resultPath))
    );
    _logger.LogInformation($"Successfully uploaded {resultPath} to {_options.Path}.");
  }

  /// <summary>
  /// Check whether a given item exists in the results store.
  /// </summary>
  /// <param name="resultPath"></param>
  /// <returns><c>true</c>if the item exists in the results store, else <c>false</c>.</returns>
  public bool ResultExists(string resultPath)
  {
    var result = Path.GetFileName(resultPath);
    return File.Exists(Path.Combine(_options.Path, result));
  }
}
