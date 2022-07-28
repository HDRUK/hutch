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

  [HttpPost]
  public async Task<IActionResult> Post([FromBody] QueryResult result)
  {
    //Endpoint for RQuest results, returns 404 when UseROCrates is set to true
    if (await _featureManager.IsEnabledAsync(Enum.GetName(FeatureFlags.UseROCrates)))
    {
      return BadRequest();
    }
    await _apiClient.ResultsEndpointPost(result.ActivitySourceId, result.JobId, result.Results);
    return Ok(_apiClient);
  }
  
  [HttpPost("rocrates")]
  public async Task<IActionResult> PostRoCrates([FromBody] ROCratesQueryResult roCratesQueryResult)
  {
    //Endpoint for ROCrates results, returns 404 when UseROCrates is set to false
    if (await _featureManager.IsEnabledAsync(Enum.GetName(FeatureFlags.UseROCrates)))
    {
      QueryResult result = new ResultsTranslator.RoCratesQueryTranslator().TranslateRoCrates(roCratesQueryResult);
      await _apiClient.ResultsEndpointPost(result.ActivitySourceId, result.JobId, result.Results);
    }
    else
    {
      return BadRequest();
    }
    return Ok(_apiClient);
  }
}
