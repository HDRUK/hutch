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
  /// 
  /// </summary>
  /// <param name="jobId"></param>
  public async Task MakeChecksums(string jobId)
  {
    throw new NotImplementedException();
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
  /// 
  /// </summary>
  /// <param name="jobId"></param>
  public async Task UploadToStore(string jobId)
  {
    throw new NotImplementedException();
  }
}
