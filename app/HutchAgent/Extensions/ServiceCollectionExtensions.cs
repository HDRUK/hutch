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
    //FileSystem // TODO Restore Filesystem Store
  }

  /// <summary>
  /// Parse the desired Results Store Provider from configuration.
  /// </summary>
  /// <param name="c">The configuration object</param>
  /// <returns>The <c>enum</c> for the chosen Results Store Provider</returns>
  private static ResultsStoreProviders GetIntermediaryStoreProvider(IConfiguration c)
  {
    Enum.TryParse<ResultsStoreProviders>(
      c["ResultsStore:Provider"],
      ignoreCase: true,
      out var resultsStoreProviders);

    return resultsStoreProviders;
  }

  private static IServiceCollection ConfigureIntermediaryStore(
    this IServiceCollection s,
    ResultsStoreProviders provider,
    IConfiguration c)
  {
    // set the appropriate configuration function
    Func<IConfiguration, IServiceCollection> configureResultsStore =
      provider switch
      {
        ResultsStoreProviders.Minio => s.Configure<MinioOptions>,
        //ResultsStoreProviders.FileSystem => s.Configure<FileSystemResultsStoreOptions>, // TODO Restore Filesystem Store
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
  public static IServiceCollection AddIntermediaryStoreFactory(this IServiceCollection s, IConfiguration c)
  {
    return s.Configure<MinioOptions>(c.GetSection("StoreDefaults"))
      .AddTransient<MinioStoreServiceFactory>();
    // TODO Restore multiple Store types
    //var storeType = GetResultsStoreProvider(c);

    //return s.ConfigureResultsStore(storeType, c)
    // .AddTransient(  
    // typeof(IResultsStoreWriter),
    // storeType switch
    // {
    //   ResultsStoreProviders.Minio => typeof(MinioStoreWriter),
    //   ResultsStoreProviders.FileSystem => typeof(FileSystemResultsStoreService),
    //   _ => throw new ArgumentOutOfRangeException()
    // });
  }
}
