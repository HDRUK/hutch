using HutchManager.Models;
using HutchManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HutchManager.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ResultsModifiersController : ControllerBase
{
  private readonly ResultsModifierService _resultsModifier;
  public ResultsModifiersController(ResultsModifierService resultsModifier)
  {
    _resultsModifier = resultsModifier;
  }

  [HttpPost]
  public async Task<ResultsModifierModel> Create(CreateResultsModifier resultsModifier)
    => await _resultsModifier.Create(resultsModifier);

  [HttpGet("types")]
  public async Task<List<ModifierTypeModel>> GetTypes()
    => await _resultsModifier.GetTypes();

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
