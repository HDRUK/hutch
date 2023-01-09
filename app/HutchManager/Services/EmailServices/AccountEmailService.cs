using HutchManager.Models.Emails;
using HutchManager.Services.Contracts;

namespace HutchManager.Services.EmailServices;

public class AccountEmailService
{
  private readonly IEmailSender _emails;

  public AccountEmailService(IEmailSender emails)
  {
    _emails = emails;
  }

  public async Task SendAccountConfirmation(EmailAddress to, string link, string resendLink)
      => await _emails.SendEmail(
          to,
          "Emails/AccountConfirmation",
          new TokenEmailModel(
            to.Name!,
            link,
            resendLink));
}

