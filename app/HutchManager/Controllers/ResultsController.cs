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
    try
    {
      QueryResult result = new ResultsTranslator.RoCratesQueryTranslator().TranslateRoCrates(roCratesQueryResult);
      await _apiClient.ResultsEndpointPost(result.ActivitySourceId, result.JobId, result.Results);
    }
    catch (InvalidDataException)
    {
      _logger.LogError("Unable to translate availability query. Data invalid");
      return BadRequest(_apiClient);
    }
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
    try
    {
      DistributionQueryTaskResult result =
        new ResultsTranslator.RoCratesToDistribution().TranslateRoCrates(roCratesQueryResult);
      await _apiClient.DistributionResultsEndpoint(result.ActivitySourceId, result.JobId, result);
    }
    catch (InvalidDataException)
    {
      _logger.LogError("Unable to translate distribution query. Data invalid");
      return BadRequest(_apiClient);
    }
    return Ok(_apiClient);
  }
}
