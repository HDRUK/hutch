using LinkLiteManager.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkLiteManager.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ActivitySourceController : Controller
{

  [HttpGet]
  public async Task<ActionResult<List<ActivitySource>>> List()
    => await _departments.List();
  /*
  public ActionResult GetProduct(string id)
  {
    return ControllerContext.
  }
  */

}
