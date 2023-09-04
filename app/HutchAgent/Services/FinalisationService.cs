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
}
