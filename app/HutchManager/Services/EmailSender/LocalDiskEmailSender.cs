using HutchManager.Config;
using HutchManager.Models.Emails;
using HutchManager.Services.Contracts;
using HutchManager.Services.EmailServices;
using Microsoft.Extensions.Options;
using MimeKit;

namespace HutchManager.Services.EmailSender
{
  public class LocalDiskEmailSender : IEmailSender
  {
    private readonly LocalDiskEmailOptions _config;
    private readonly RazorViewService _emailViews;

    public LocalDiskEmailSender(
        IOptions<LocalDiskEmailOptions> options,
        RazorViewService emailViews)
    {
      _config = options.Value;
      _emailViews = emailViews;
    }

    /// <inheritdoc />
    public async Task SendEmail<TModel>(List<EmailAddress> toAddresses, string viewName, TModel model)
        where TModel : class
    {
      var (body, viewContext) = await _emailViews.RenderToString(viewName, model);

      var message = new MimeMessage();

      foreach (var address in toAddresses)
        message.To.Add(!string.IsNullOrEmpty(address.Name)
            ? new MailboxAddress(address.Name, address.Address)
            : MailboxAddress.Parse(address.Address));

      message.From.Add(new MailboxAddress(_config.FromName, _config.FromAddress));
      message.ReplyTo.Add(MailboxAddress.Parse(_config.ReplyToAddress));
      message.Subject = (string?)viewContext.ViewBag.Subject ?? string.Empty;

      message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
      {
        Text = body
      };

      await message.WriteToAsync(
          Path.Combine(_config.LocalPath,
              MessageFileName(viewName, toAddresses[0].Address)));
    }

    public async Task SendEmail<TModel>(EmailAddress toAddress, string viewName, TModel model)
        where TModel : class
        => await SendEmail(new List<EmailAddress> { toAddress }, viewName, model);

    private static string ShortViewName(string viewName)
        => viewName[(viewName.LastIndexOf('/') + 1)..];

    private static string SafeIsoDate(DateTimeOffset date)
        => date.ToString("o").Replace(":", "-");

    private static string MessageFileName(string viewName, string recipient)
        => $"{ShortViewName(viewName)}_{recipient}_{SafeIsoDate(DateTimeOffset.UtcNow)}.eml";
  }
}
