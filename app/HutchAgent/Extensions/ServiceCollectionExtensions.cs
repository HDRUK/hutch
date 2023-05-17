using HutchAgent.Config;
using HutchAgent.Services;

namespace HutchAgent.Extensions;

public static class ServiceCollectionExtensions
{
  /// <summary>
  /// Enum for Results Store Providers.
  /// </summary>
  enum ResultsStoreProviders
  {
    Minio,
    FileSystem
  }

  /// <summary>
  /// Parse the desired Results Store Provider from configuration.
  /// </summary>
  /// <param name="c">The configuration object</param>
  /// <returns>The <c>enum</c> for the chosen Results Store Provider</returns>
  private static ResultsStoreProviders GetResultsStoreProvider(IConfiguration c)
  {
    Enum.TryParse<ResultsStoreProviders>(
      c["ResultsStore:Provider"],
      ignoreCase: true,
      out var resultsStoreProviders);

    return resultsStoreProviders;
  }

  private static IServiceCollection ConfigureResultsStore(
    this IServiceCollection s,
    ResultsStoreProviders provider,
    IConfiguration c)
  {
    // set the appropriate configuration function
    Func<IConfiguration, IServiceCollection> configureResultsStore =
      provider switch
      {
        ResultsStoreProviders.Minio => s.Configure<MinioOptions>,
        ResultsStoreProviders.FileSystem => s.Configure<FileSystemResultsStoreOptions>,
        _ => throw new ArgumentOutOfRangeException(nameof(provider), provider, null)
      };

    // and execute it
    configureResultsStore.Invoke(c.GetSection("ResultsStore"));

    return s;
  }

  /// <summary>
  /// Determine which results store the user wishes to use and adds it the service collection.
  /// </summary>
  /// <param name="s"></param>
  /// <param name="c"></param>
  /// <returns></returns>
  public static IServiceCollection AddResultsStore(this IServiceCollection s, IConfiguration c)
  {
    var storeType = GetResultsStoreProvider(c);

    return s.ConfigureResultsStore(storeType, c)
      .AddTransient(
        typeof(IResultsStoreWriter),
        storeType switch
        {
          ResultsStoreProviders.Minio => typeof(MinioService),
          ResultsStoreProviders.FileSystem => typeof(FileSystemResultsStoreService),
          _ => throw new ArgumentOutOfRangeException()
        });
  }
}
