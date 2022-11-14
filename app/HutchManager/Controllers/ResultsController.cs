using HutchManager.Constants;
using HutchManager.Dto;
using HutchManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;

namespace HutchManager.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class ResultsController: ControllerBase
{
  private readonly ILogger<ResultsController> _logger;
  private readonly RQuestTaskApiClient _apiClient;
  private readonly IFeatureManager _featureManager;

  public ResultsController(ILogger<ResultsController> logger, RQuestTaskApiClient apiClient, IFeatureManager featureManager)
  {
    _logger = logger;
    _apiClient = apiClient;
    _featureManager = featureManager;
  }
  
  /// <summary>
  /// Endpoint for ROCrates availability results.
  /// </summary>
  /// <param name="roCratesQueryResult"></param>
  /// <returns></returns>
  [HttpPost("availability")]
  public async Task<IActionResult> PostRoCrates([FromBody] ROCratesQueryResult roCratesQueryResult)
  {
    QueryResult result = new ResultsTranslator.RoCratesQueryTranslator().TranslateRoCrates(roCratesQueryResult);
    await _apiClient.ResultsEndpointPost(result.ActivitySourceId, result.JobId, result.Results);
    return Ok(_apiClient);
  }
  
  /// <summary>
  /// Endpoint for ROCrates distribution results.
  /// </summary>
  /// <param name="roCratesQueryResult"></param>
  /// <returns></returns>
  [HttpPost("distribution")]
  public async Task<IActionResult> PostDistributionResults([FromBody] ROCratesQueryResult roCratesQueryResult)
  {
    DistributionQueryTaskResult result =
      new ResultsTranslator.RoCratesToDistribution().TranslateRoCrates(roCratesQueryResult);
      await _apiClient.DistributionResultsEndpoint(result.ActivitySourceId, result.JobId, result);
      return Ok(_apiClient);
  }
}
