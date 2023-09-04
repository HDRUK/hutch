using System.IO.Compression;

namespace HutchAgent.Services;

public class FinalisationService
{
  private readonly BagItService _bagItService;
  private readonly CrateService _crateService;
  private readonly ILogger<FinalisationService> _logger;
  private readonly IResultsStoreWriter _storeWriter;
  private readonly WorkflowJobService _jobService;

  public FinalisationService(
    BagItService bagItService,
    CrateService crateService,
    ILogger<FinalisationService> logger,
    IResultsStoreWriter storeWriter, WorkflowJobService jobService)
  {
    _bagItService = bagItService;
    _crateService = crateService;
    _logger = logger;
    _storeWriter = storeWriter;
    _jobService = jobService;
  }

  /// <summary>
  /// Merge a result crate from a workflow run back into its input crate.
  /// </summary>
  /// <param name="jobId"></param>
  public async Task MergeCrate(string jobId)
  {
    throw new NotImplementedException();
  }

  /// <summary>
  /// Make the checksums for the BagIt containing the job's data.
  /// </summary>
  /// <param name="jobId">The ID of the job needing checksums.</param>
  public async Task MakeChecksums(string jobId)
  {
    var job = await _jobService.Get(jobId);
    await _bagItService.WriteManifestSha512(job.WorkingDirectory);
    await _bagItService.WriteTagManifestSha512(job.WorkingDirectory);
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="jobId"></param>
  public async Task UpdateMetadata(string jobId)
  {
    throw new NotImplementedException();
  }

  /// <summary>
  /// Zip a workflow job's BagIt and add it to the configured results store.
  /// </summary>
  /// <param name="jobId"></param>
  /// <exception cref="DirectoryNotFoundException">Could not locate the parent directory of the job's BagIt.</exception>
  public async Task UploadToStore(string jobId)
  {
    var job = await _jobService.Get(jobId);
    var bagitDir = new DirectoryInfo(job.WorkingDirectory);
    var bagitParent = bagitDir.Parent ??
                      throw new DirectoryNotFoundException($"Cannot find parent directory of {bagitDir.FullName}");
    var pathToZip = Path.Combine(
      bagitParent.FullName,
      Path.TrimEndingDirectorySeparator(bagitDir.FullName) + ".zip"
    );

    // Zip the BagIt before upload
    ZipFile.CreateFromDirectory(bagitDir.FullName, pathToZip);
    var zipFile = new FileInfo(pathToZip);

    // Make sure results store exists
    if (!await _storeWriter.StoreExists())
    {
      _logger.LogCritical("Could not write {} to the results store. Store not found.", zipFile.Name);
      return;
    }

    // Check if the result already exists
    if (await _storeWriter.ResultExists(zipFile.Name))
    {
      _logger.LogInformation(
        "{} already exists in results store, overwriting.", zipFile.Name);
    }

    // Upload the zipped BagIt
    _logger.LogInformation("Adding {} to results store.", zipFile.Name);
    await _storeWriter.WriteToStore(pathToZip);

    // Delete the zipped BagIt to save disk space
    zipFile.Delete();
  }
}
