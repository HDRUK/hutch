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
  private readonly DataSourceService _dataSources;
  public AgentsController(DataSourceService dataSources)
  {
    _dataSources = dataSources;
  }

  [HttpPost("checkin")]
  public async Task<IActionResult> CheckIn(AgentCheckInModel payload)
  {
    foreach (var ds in payload.DataSources) 
      await _dataSources.CreateOrUpdate(new() { Id = ds });

    return Accepted();
  }

}
