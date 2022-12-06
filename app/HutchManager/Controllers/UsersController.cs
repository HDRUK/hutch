using HutchManager.Models.User;
using HutchManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HutchManager.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{

  private readonly UserService _user;

  public UsersController(UserService user)
  {
    _user = user;
  }

  [HttpGet]
  public async Task<List<UserModel>> List()
    => await _user.List();

  [HttpPost]
  public async Task<IActionResult> Create([FromBody] UserModel user)
  {
    await _user.Create(user);
    return Ok(user);
  }
}

