using System.Text.Json;
using HutchManager.Auth;
using HutchManager.Data.Entities.Identity;
using HutchManager.Models.Account;
using HutchManager.Models.User;
using HutchManager.Services;
using HutchManager.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HutchManager.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
  private readonly UserManager<ApplicationUser> _users;
  private readonly SignInManager<ApplicationUser> _signIn;
  private readonly UserService _user;
  private readonly TokenIssuingService _tokens;

  public AccountController(
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

  [HttpPost("register")]
  public async Task<IActionResult> Register(RegisterAccountModel model)
  {
    RegisterAccountResult regResult = new();

    if (ModelState.IsValid) // Additional Pre-registration checks
    {
      if (!await _user.CanRegister(model.Email))
      {
        ModelState.AddModelError(string.Empty, "The email address provided is not eligible for registration.");
        regResult = regResult with
        {
          IsNotAllowlisted = true,
        };
      }
    }

    if (ModelState.IsValid) // Actual success route
    {
      var user = new ApplicationUser
      {
        UserName = model.Email,
        Email = model.Email,
        FullName = model.FullName,
        UICulture = Request.GetUICulture().Name
      };

      var result = await _users.CreateAsync(user, model.Password);
      if (result.Succeeded)
      {
        if (!_users.Options.SignIn.RequireConfirmedEmail) return NoContent(); // 204 status
        
        await _tokens.SendAccountConfirmation(user); // If email confirmation required is true, then only send confirmation 
        return Accepted(); // 202 status 
      }

      foreach (var e in result.Errors)
      {
        ModelState.AddModelError(string.Empty, e.Description);

        if (new[] { "DuplicateEmail", "DuplicateUserName" }.Contains(e.Code))
        {
          var existingUser = await _users.FindByEmailAsync(model.Email);

          if (existingUser is not null)
            regResult = regResult with
            {
              IsExistingUser = true
            };
        }
      }
    }

    return BadRequest(regResult with
    {
      Errors = ModelState.CollapseErrors()
    });
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login(LoginModel model)
  {
    if (ModelState.IsValid)
    {
      var result = await _signIn.PasswordSignInAsync(model.Username, model.Password, false, true);
      var user = await _users.FindByNameAsync(model.Username);

      if (result.Succeeded)
      {
        if (user is null)
          throw new InvalidOperationException(
            $"Successfully signed in user could not be retrieved! Username: {model.Username}");

        var profile = await _user.BuildProfile(user);

        // Write a basic Profile Cookie for JS
        HttpContext.Response.Cookies.Append(
          AuthConfiguration.ProfileCookieName,
          JsonSerializer.Serialize((BaseUserProfileModel)profile),
          AuthConfiguration.ProfileCookieOptions);

        return Ok(new LoginResult
        {
          User = profile,
        });
      }

      // Handle login failures
      if (result.IsNotAllowed)
      {
        // But WHY was it disallowed?
        // Distinguish some specific cases we care about
        // So the login form can behave accordingly

        LoginResult loginResult = user switch
        {
          { EmailConfirmed: false } => new() { IsUnconfirmedAccount = true },
          _ => new() { }
        };

        return BadRequest(loginResult);
      }
    }

    return BadRequest(new LoginResult
    {
      Errors = ModelState.CollapseErrors()
    });
  }

  [HttpPost("logout")]
  public async Task Logout()
  {
    // Sign out of Identity
    await _signIn.SignOutAsync();

    // Also remove the JS Profile Cookie
    HttpContext.Response.Cookies.Delete(AuthConfiguration.ProfileCookieName);
  }

  [HttpPost("confirm")]
  public async Task<IActionResult> Confirm(UserTokenModel model)
  {
    if (ModelState.IsValid)
    {
      var user = await _users.FindByIdAsync(model.UserId);
      if (user is null) return NotFound();

      var result = await _users.ConfirmEmailAsync(user, model.Token);

      if (!result.Errors.Any())
      {
        await _signIn.SignInAsync(user, false);

        var profile = await _user.BuildProfile(user);

        // Write a basic Profile Cookie for JS
        HttpContext.Response.Cookies.Append(
          AuthConfiguration.ProfileCookieName,
          JsonSerializer.Serialize((BaseUserProfileModel)profile),
          AuthConfiguration.ProfileCookieOptions);

        return Ok(profile);
      }
    }

    return BadRequest();
  }

  [HttpPost("confirm/resend")]
  public async Task<IActionResult> ConfirmResend([FromBody] string userIdOrEmail)
  {
    var user = await _users.FindByIdAsync(userIdOrEmail);
    if (user is null) user = await _users.FindByEmailAsync(userIdOrEmail);
    if (user is null) return NotFound();

    await _tokens.SendAccountConfirmation(user);
    return NoContent();
  }

  [HttpPost("password/reset")]
  public async Task<IActionResult> RequestPasswordReset([FromBody] string userIdOrEmail)
  {
    var user = await _users.FindByIdAsync(userIdOrEmail);
    if (user is null) user = await _users.FindByEmailAsync(userIdOrEmail);
    if (user is null) return NotFound();

    await _tokens.SendPasswordReset(user);
    return NoContent();
  }

  [HttpPost("password")]
  public async Task<IActionResult> ResetPassword(AnonymousSetPasswordModel model)
  {
    if (ModelState.IsValid)
    {
      var user = await _users.FindByIdAsync(model.Credentials.UserId);
      if (user is null) return NotFound();

      var result = await _users.ResetPasswordAsync(user, model.Credentials.Token, model.Data.Password);

      if (!result.Errors.Any())
      {
        if (user.EmailConfirmed)
        {
          await _signIn.SignInAsync(user, false);

          var profile = await _user.BuildProfile(user);

          // Write a basic Profile Cookie for JS
          HttpContext.Response.Cookies.Append(
            AuthConfiguration.ProfileCookieName,
            JsonSerializer.Serialize((BaseUserProfileModel)profile),
            AuthConfiguration.ProfileCookieOptions);

          return Ok(new SetPasswordResult
          {
            User = profile
          });
        }
        else
        {
          return Ok(new SetPasswordResult
          {
            IsUnconfirmedAccount = true
          });
        }
      }
    }

    return BadRequest(new SetPasswordResult
    {
      Errors = ModelState.CollapseErrors()
    });
  }
  
  [HttpPost("activate")]
  public async Task<IActionResult> Activate (UserTokenModel model)
  {
    if (ModelState.IsValid)
    {
      var user = await _users.FindByIdAsync(model.UserId);
      if (user is null) return NotFound();

      var isTokenValid = await _users.VerifyUserTokenAsync(user, "Default", "ActivateAccount", model.Token); // validate token

      if (!isTokenValid) return BadRequest();
      
      user.AccountConfirmed = true;
      await _users.UpdateAsync(user);
      await _signIn.SignInAsync(user, false);
      var profile = await _user.BuildProfile(user);
      // Write a basic Profile Cookie for JS
      HttpContext.Response.Cookies.Append(
        AuthConfiguration.ProfileCookieName,
        JsonSerializer.Serialize((BaseUserProfileModel)profile),
        AuthConfiguration.ProfileCookieOptions);
      return Ok(profile);
    }
    return BadRequest();
  }
}
