using HutchManager.Models;
using HutchManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HutchManager.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ActivitySourcesController : ControllerBase
{
  private readonly ActivitySourceService _activitySources;
  public ActivitySourcesController(ActivitySourceService activitySources)
  {
    _activitySources = activitySources;
  }

  [HttpGet]
  public async Task<List<Models.ActivitySourceModel>> List()
    => await _activitySources.List();
  
  [HttpPost]
  public async Task<ActivitySourceModel> Create(CreateActivitySource activitySource)
    => await _activitySources.Create(activitySource);
  
  [HttpGet("{id}")]
  public async Task<ActionResult<ActivitySourceModel>> Get(int id)
    =>await _activitySources.Get(id);
  
  [HttpGet("{id}/resultsmodifiers")]
  public async Task<List<Models.ResultsModifierModel>> GetActivitySourceResultsModifier(int id)
    => await _activitySources.GetActivitySourceResultsModifier(id);
  
  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(int id)
  {
    try
    {
      await _activitySources.Delete(id);
    }
    catch (KeyNotFoundException) {
      
    }
    return NoContent();
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> Set(int id, [FromBody] CreateActivitySource activitySource)
  {
    try
    {
      return Ok(await _activitySources.Set(id, activitySource));
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
}
