using System.Globalization;
using System.Text.Json;
using HutchManager.Auth;
using HutchManager.Data.Entities.Identity;
using HutchManager.Models.User;
using HutchManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HutchManager.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
  private readonly UserManager<ApplicationUser> _users;
  private readonly SignInManager<ApplicationUser> _signIn;
  private readonly UserService _user;
  private readonly TokenIssuingService _tokens;

  public UserController(
    UserManager<ApplicationUser> users,
    SignInManager<ApplicationUser> signIn,
    UserService user,
    TokenIssuingService tokens)
  {
    _users = users;
    _signIn = signIn;
    _user = user;
    _tokens = tokens;
  }

  [HttpGet("me")]
  public async Task<IActionResult> Me()
  {
    var profile = await _user.BuildProfile(User);
    return Ok(profile);
  }

  [HttpPut("uiCulture")]
  public async Task<IActionResult> SetUICulture([FromBody] string culture)
  {
    try
    {
      var user = await _users.FindByNameAsync(User.Identity?.Name);

      // Save it
      await _user.SetUICulture(user.Id, culture);

      // Sign In again to reset user cookie
      await _signIn.SignInAsync(user, false);

      var profile = await _user.BuildProfile(user);

      // Write a basic Profile Cookie for JS
      HttpContext.Response.Cookies.Append(
        AuthConfiguration.ProfileCookieName,
        JsonSerializer.Serialize((BaseUserProfileModel)profile),
        AuthConfiguration.ProfileCookieOptions);
    }
    catch (KeyNotFoundException) { return NotFound(); }
    catch (CultureNotFoundException) { return BadRequest(); }

    return NoContent();
  }

  [HttpPost]
  public async Task<IActionResult> Create([FromBody] UserModel user)
  {
    await _user.Create(user);
    return Ok(user);
  }
  
  [HttpPost("{userIdOrEmail}/activation")] //api/users/{userIdOrEmail}/activation
  public async Task<IActionResult> GenerateAccountActivationLink(string userIdOrEmail)
  {
    var user = await _users.FindByIdAsync(userIdOrEmail);
    if (user is null) user = await _users.FindByEmailAsync(userIdOrEmail);
    if (user is null) return NotFound();
    return Ok(await _tokens.GenerateAccountActivationLink(user));
  }
}

