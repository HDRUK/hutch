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
  private readonly ResultsModifierService _resultsModifier;

  public ActivitySourcesController(ActivitySourceService activitySources, ResultsModifierService resultsModifier)
  {
    _activitySources = activitySources;
    _resultsModifier = resultsModifier;
  }

  /// <summary>
  /// Get a list of all ActivitySources
  /// </summary>
  /// <returns></returns>
  [HttpGet]
  public async Task<List<ActivitySourceModel>> List()
    => await _activitySources.List();

  /// <summary>
  /// Create a new ActivitySource
  /// </summary>
  /// <param name="activitySource"></param>
  /// <returns></returns>
  [HttpPost]
  public async Task<ActivitySourceModel> Create(CreateActivitySource activitySource)
    => await _activitySources.Create(activitySource);

  /// <summary>
  /// Get an ActivitySource by ID
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  [HttpGet("{id}")]
  public async Task<ActionResult<ActivitySourceModel>> Get(int id)
    => await _activitySources.Get(id);

  /// <summary>
  /// Get all ResultsModifiers related to an ActivitySource
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  [HttpGet("{id}/resultsmodifiers")]
  public async Task<List<Models.ResultsModifierModel>> GetActivitySourceResultsModifiers(int id)
    => await _activitySources.GetActivitySourceResultsModifiers(id);

  /// <summary>
  /// Delete an ActivitySource by ID
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(int id)
  {
    try
    {
      await _activitySources.Delete(id);
    }
    catch (KeyNotFoundException)
    {

    }
    return NoContent();
  }

  /// <summary>
  /// Modify an ActivitySource by ID
  /// </summary>
  /// <param name="id"></param>
  /// <param name="activitySource"></param>
  /// <returns></returns>
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

  [HttpPost("{id}/resultsmodifiers")]
  public async Task<IActionResult> Create(int id, CreateResultsModifier resultsModifier)
  {
    try
    {
        return Ok(await _resultsModifier.Create(id, resultsModifier));
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
 
   
}
