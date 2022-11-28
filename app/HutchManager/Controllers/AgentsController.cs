using HutchManager.Models;
using HutchManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HutchManager.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AgentsController : ControllerBase
{
  private readonly AgentService _agents;
  private readonly DataSourceService _dataSources;
  public AgentsController(AgentService agents,DataSourceService dataSources)
  {
    _dataSources = dataSources;
    _agents = agents;
  }
  
  [HttpPost("checkin")]
  public async Task<IActionResult> CheckIn(AgentCheckInModel payload)
  {
    foreach (var ds in payload.DataSources) 
      await _dataSources.CreateOrUpdate(new() { Id = ds },payload.Agents);
    return Accepted();
  }

  [HttpPost]
  public async Task<ManageAgent> Create(ManageAgent manageAgent)
    => await _agents.Create(manageAgent);
  
  /// <summary>
  /// Get a Agents list
  /// </summary>
  /// <returns></returns>
  [HttpGet]
  public async Task<List<ManageAgent>> List()
    => await _agents.List();
  
  /// <summary>
  /// Get an Agent by ID
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  [HttpGet("{id}")]
  public async Task<ActionResult<ManageAgent>> Get(int id)
    => await _agents.Get(id);

  [HttpPut("{id}")]
  public async Task<IActionResult> Set(int id, [FromBody] ManageAgent agent)
  {
    try
    {
      return Ok(await _agents.Set(id, agent));
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  
  /// <summary>
  /// Delete an Agent by ID
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(int id)
  {
    try
    {
      await _agents.Delete(id);
    }
    catch (KeyNotFoundException)
    {

    }
    return NoContent();
  }
  
  [HttpPut("{id}/secret")] //api/agents/{id}/secret
  public async Task<ManageAgent> Set (int id)
    =>  await _agents.GenerateNewSecret(id);
  
}
