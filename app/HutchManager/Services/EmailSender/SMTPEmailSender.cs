using HutchManager.Config;
using HutchManager.Models.Emails;
using HutchManager.Services.Contracts;
using HutchManager.Services.EmailServices;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace HutchManager.Services.EmailSender;

public class SmtpEmailSender : IEmailSender
{
  private readonly SmtpOptions _config;
  private readonly RazorViewService _emailViews;
  private readonly SmtpClient _smtpClient;
  
  public SmtpEmailSender(
    IOptions<SmtpOptions> options,
    RazorViewService emailViews)
  {
    _config = options.Value;
    _emailViews = emailViews;
    _smtpClient = new SmtpClient();
    
  }

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

    var secureSocketOption = new Dictionary<int, SecureSocketOptions>
    {
      { 0, SecureSocketOptions.None },
      { 1, SecureSocketOptions.Auto },
      { 2, SecureSocketOptions.SslOnConnect },
      { 3, SecureSocketOptions.StartTls },
      { 4, SecureSocketOptions.StartTlsWhenAvailable }
    };
    try
    {
      await _smtpClient.ConnectAsync(_config.SmtpHost, _config.SmtpPort,
        secureSocketOption[_config.SmtpSecureSocketEnum]);
      await _smtpClient.AuthenticateAsync(_config.SmtpUsername, _config.SmtpPassword);

    }
    catch (Exception ex)
    {
      throw new InvalidOperationException(ex.ToString());
    }
    finally
    {
      await _smtpClient.DisconnectAsync(true);
      _smtpClient.Dispose();
    }
  }

  public async Task SendEmail<TModel>(EmailAddress toAddress, string viewName, TModel model)
    where TModel : class
    => await SendEmail(new List<EmailAddress> { toAddress }, viewName, model);


}
