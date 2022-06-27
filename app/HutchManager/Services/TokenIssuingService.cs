using HutchManager.Constants;
using HutchManager.Data.Entities.Identity;
using HutchManager.Models.Account;
using HutchManager.Models.Emails;
using HutchManager.Services.EmailServices;
using HutchManager.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace HutchManager.Services;

public class TokenIssuingService
{
  private readonly ActionContext _actionContext;
  private readonly UserManager<ApplicationUser> _users;
  private readonly AccountEmailService _accountEmail;

  public TokenIssuingService(
      IActionContextAccessor actionContextAccessor,
      UserManager<ApplicationUser> users,
      AccountEmailService accountEmail)
  {
    _users = users;
    _accountEmail = accountEmail;
    _actionContext = actionContextAccessor.ActionContext
      ?? throw new InvalidOperationException("Failed to get the ActionContext");
  }

  /// <summary>
  /// Issue an AccountConfirmation token, and email the user a link.
  /// </summary>
  /// <param name="user">The user to issue the token for and send the email to.</param>
  public async Task SendAccountConfirmation(ApplicationUser user)
  {
    var token = await _users.GenerateEmailConfirmationTokenAsync(user);
    var vm = new UserTokenModel(user.Id, token);

    await _accountEmail.SendAccountConfirmation(
        new EmailAddress(user.Email)
        {
          Name = user.FullName
        },
        link: (ClientRoutes.ConfirmAccount +
            $"?vm={vm.ObjectToBase64UrlJson()}")
            .ToLocalUrlString(_actionContext.HttpContext.Request),
        resendLink: (ClientRoutes.ResendConfirm +
            $"?vm={new { UserId = user.Id }.ObjectToBase64UrlJson()}")
            .ToLocalUrlString(_actionContext.HttpContext.Request));
  }

  public async Task SendPasswordReset(ApplicationUser user)
  {
    var token = await _users.GeneratePasswordResetTokenAsync(user);
    var vm = new UserTokenModel(user.Id, token);

    await _accountEmail.SendPasswordReset(
        new EmailAddress(user.Email)
        {
          Name = user.FullName
        },
        link: (ClientRoutes.ResetPassword +
            $"?vm={vm.ObjectToBase64UrlJson()}")
            .ToLocalUrlString(_actionContext.HttpContext.Request),
        resendLink: (ClientRoutes.ResendResetPassword +
            $"?vm={new { UserId = user.Id }.ObjectToBase64UrlJson()}")
            .ToLocalUrlString(_actionContext.HttpContext.Request));
  }

  public async Task SendEmailChange(ApplicationUser user, string newEmail)
  {
    var token = await _users.GenerateChangeEmailTokenAsync(user, newEmail);
    var vm = new UserTokenModel(user.Id, token);

    // TODO: do we need the new email in the VM?

    await _accountEmail.SendEmailChange(
        new EmailAddress(newEmail)
        {
          Name = user.FullName
        },
        link: (ClientRoutes.ConfirmEmailChange +
        $"?vm={vm.ObjectToBase64UrlJson()}")
            .ToLocalUrlString(_actionContext.HttpContext.Request),
        resendLink: (ClientRoutes.ResendConfirm + // TODO
            $"?vm={new { UserId = user.Id }.ObjectToBase64UrlJson()}")
            .ToLocalUrlString(_actionContext.HttpContext.Request));
  }
}

