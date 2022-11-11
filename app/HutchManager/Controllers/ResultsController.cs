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
    
    // Get activitySourceId from results
    int? activitySourceId = null;
    foreach (var o in roCratesQueryResult.Graphs)
    {
      var g = (ROCratesGraph)o;
      if (g.Name != "activity_source_id") continue;
      activitySourceId = int.Parse(g.Value);
      break;
    }

    if (activitySourceId != null)
    {
      await _apiClient.DistributionResultsEndpoint(activitySourceId.Value, result.JobId, result);
      return Ok(_apiClient);
    }
    
    return BadRequest(_apiClient);
  }
}
