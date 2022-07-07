using HutchManager.Models;
using HutchManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HutchManager.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ResultsModifierController : ControllerBase
{
  private readonly ResultsModifierService _resultsModifier;
  public ResultsModifierController(ResultsModifierService resultsModifier)
  {
    _resultsModifier = resultsModifier;
  }

  [HttpGet]
  public async Task<List<Models.ResultsModifierModel>> List()
    => await _resultsModifier.List();

  [HttpPost]
  public async Task<ResultsModifierModel> Create(CreateResultsModifier resultsModifier)
    => await _resultsModifier.Create(resultsModifier);

  [HttpGet("{id}")]
  public async Task<ActionResult<ResultsModifierModel>> Get(int id)
    => await _resultsModifier.Get(id);

  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(int id)
  {
    try
    {
      await _resultsModifier.Delete(id);
    }
    catch (KeyNotFoundException)
    {

    }
    return NoContent();
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> Set(int id, [FromBody] CreateResultsModifier resultsModifier)
  {
    try
    {
      return Ok(await _resultsModifier.Set(id, resultsModifier));
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
}
