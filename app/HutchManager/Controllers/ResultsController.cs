using System.Text.Json;
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
  /// Endpoint for results.
  /// </summary>
  /// <param name="body"></param>
  /// <returns></returns>
  [HttpPost]
  public async Task<IActionResult> PostRoCrates([FromBody] ActivityJob body)
  {
    var result = JsonSerializer.Deserialize<RquestQueryResult>(body.Payload);
    if (result != null)
    {
      await _apiClient.ResultsEndpointPost(body.ActivitySourceId, body.JobId, result);
      return Ok(_apiClient);
    }
    // JSON sent to manager is not valid.
    return BadRequest(_apiClient);
  }
}
