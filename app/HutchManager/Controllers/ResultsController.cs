using HutchManager.Dto;
using HutchManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HutchManager.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ResultsController: ControllerBase
{
  private readonly ILogger<ResultsController> _logger;
  private readonly RquestTaskApiClient _apiClient;

  public ResultsController(ILogger<ResultsController> logger,RquestTaskApiClient apiClient)
  {
    _logger = logger;
    _apiClient = apiClient;
  }
  [HttpPost]
  public async Task<IActionResult> Post([FromBody] QueryResult result)
  {
    await _apiClient.ResultsEndpointPost(result.ActivitySourceId, result.JobId, result.Results);
    return Ok(_apiClient);
  }

}
