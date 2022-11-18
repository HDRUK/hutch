using HutchManager.Models;
using HutchManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HutchManager.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AgentsController : ControllerBase
{
  private readonly DataSourceService _dataSources;
  private readonly AgentService _agents;
  
  public AgentsController(DataSourceService dataSources, AgentService agents)
  {
    _dataSources = dataSources;
    _agents = agents;
  }

  [AllowAnonymous]
  [HttpPost("checkin")]
  public async Task<IActionResult> CheckIn(AgentCheckInModel payload)
  {
    foreach (var ds in payload.DataSources) 
      await _dataSources.CreateOrUpdate(new() { Id = ds });

    return Accepted();
  }

  /// <summary>
  /// Get a Agents list
  /// </summary>
  /// <returns></returns>
  [HttpGet]
  public async Task<List<AgentResultsModel>> List()
    => await _agents.List();
  
  /// <summary>
  /// Get an Agent by ID
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  [HttpGet("{id}")]
  public async Task<ActionResult<AgentResultsModel>> Get(int id)
    => await _agents.Get(id);
}
