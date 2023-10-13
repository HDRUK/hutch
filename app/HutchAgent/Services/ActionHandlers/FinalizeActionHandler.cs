using System.IO.Compression;
using System.Text.Json;
using HutchAgent.Config;
using HutchAgent.Constants;
using HutchAgent.Models;
using HutchAgent.Services.Contracts;
using HutchAgent.Utilities;
using Microsoft.FeatureManagement;

namespace HutchAgent.Services.ActionHandlers;

public class FinalizeActionHandler : IActionHandler
{
  private readonly BagItService _bagItService;
  private readonly FiveSafesCrateService _crateService;
  private readonly ILogger<FinalizeActionHandler> _logger;
  private readonly MinioStoreServiceFactory _storeFactory;
  private readonly WorkflowJobService _jobs;
  private readonly JobLifecycleService _job;
  private readonly StatusReportingService _status;
  private readonly IFeatureManager _features;
  private readonly ControllerApiService _controller;

  private const string _finalPackageFilename = "final-result-crate.zip";

  public FinalizeActionHandler(
    BagItService bagItService,
    FiveSafesCrateService crateService,
    ILogger<FinalizeActionHandler> logger,
    MinioStoreServiceFactory storeFactory,
    WorkflowJobService jobs,
    JobLifecycleService job,
    StatusReportingService status,
    IFeatureManager features,
    ControllerApiService controller)
  {
    _bagItService = bagItService;
    _crateService = crateService;
    _logger = logger;
    _storeFactory = storeFactory;
    _jobs = jobs;
    _job = job;
    _status = status;
    _features = features;
    _controller = controller;
  }

  public async Task HandleAction(string jobId, object? payload)
  {
    var job = await _jobs.Get(jobId);

    // TODO Should we be checking if the job is actually ready instead of assuming it can only be queued if it is?!
    // yes - at minimum check that the disclosure check assessaction is complete

    // 1. Copy approved outputs to the working crate
    FilesystemUtility.CopyDirectory(
      job.WorkingDirectory.JobEgressOutputs(),
      Path.Combine(job.WorkingDirectory.JobCrateRoot(), "outputs"),
      recursive: true);

    // 2. Update Crate Metadata
    _crateService.FinalizeMetadata(job);

    // 3. BagIt Checksums
    await MakeChecksums(job);

    // 4. Zip the final BagIt package
    if (!Directory.Exists(job.WorkingDirectory.JobEgressPackage()))
      Directory.CreateDirectory(job.WorkingDirectory.JobEgressPackage());

    ZipFile.CreateFromDirectory(
      job.WorkingDirectory.JobBagItRoot(),
      Path.Combine(job.WorkingDirectory.JobEgressPackage(), _finalPackageFilename));

    // 5. Upload
    var uploadPath = await UploadFinalCrate(job);

    if (!await _features.IsEnabledAsync(FeatureFlags.StandaloneMode))
    {
      // Submit FinalOutcome to Controller API
      await _controller.FinalOutcome(job.Id, uploadPath);
    }

    await _status.ReportStatus(jobId, JobStatus.Complete);

    // 6. Clean up // TODO should this be deferred to an expiry process, so we keep the artifacts for a configured amount of time?
    await _job.Cleanup(job);
  }

  /// <summary>
  /// Make the checksums for the BagIt containing the job's data.
  /// </summary>
  /// <param name="job">The job needing checksums.</param>
  private async Task MakeChecksums(WorkflowJob job)
  {
    await _bagItService.WriteManifestSha512(job.WorkingDirectory.JobBagItRoot());
    await _bagItService.WriteTagManifestSha512(job.WorkingDirectory.JobBagItRoot());
  }

  /// <summary>
  /// Zip a workflow job's BagIt and add it to the configured results store.
  /// </summary>
  /// <param name="job"></param>
  /// <exception cref="DirectoryNotFoundException">Could not locate the parent directory of the job's BagIt.</exception>
  /// <returns>The Intermediary Store Object ID for the Final Results Package</returns>
  private async Task<string> UploadFinalCrate(WorkflowJob job)
  {
    if (string.IsNullOrWhiteSpace(job.EgressTarget))
      throw new InvalidOperationException(
        $"Finalized Job {job.Id} cannot be egressed as no egress target was recorded.");

    var egressTarget = JsonSerializer.Deserialize<MinioOptions>(job.EgressTarget);
    var store = await _storeFactory.Create(egressTarget);

    var finalObjectId = await _features.IsEnabledAsync(FeatureFlags.StandaloneMode)
      ? Path.Combine(job.Id, _finalPackageFilename)
      : _finalPackageFilename;

    // Make sure results store exists
    if (!await store.StoreExists())
    {
      _logger.LogCritical(
        "Could not write '{Package}' to the store: Store not found",
        finalObjectId);

      throw new InvalidOperationException("Couldn't find provided intermediary store.");
    }

    // Upload the zipped BagIt
    _logger.LogDebug("Adding Final Results '{Package}' to intermediary store", finalObjectId);
    await store.WriteToStore(
      Path.Combine(
        job.WorkingDirectory.JobEgressPackage(),
        _finalPackageFilename),
      finalObjectId);

    return finalObjectId;
  }
}
