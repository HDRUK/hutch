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
  [HttpPost("{jobId}")]
  public async Task<IActionResult> Post([FromBody] int activitySourceId,string jobId, int? count = null)
  {
    await _apiClient.ResultsEndpointPost(activitySourceId, jobId, count);
    return NoContent();
  }

}
