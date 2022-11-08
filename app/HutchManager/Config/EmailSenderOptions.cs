namespace HutchManager.Config
{
  public record BaseEmailSenderOptions
  {
    public BaseEmailSenderOptions()
    {
      if (string.IsNullOrWhiteSpace(ReplyToAddress))
        ReplyToAddress = FromAddress;
    }

    public string ServiceName { get; init; } = "Hutch";
    public string FromName { get; init; } = "No Reply";
    public string FromAddress { get; init; } = "noreply@example.com";
    public string ReplyToAddress { get; init; } = string.Empty;
  };

  public record LocalDiskEmailOptions : BaseEmailSenderOptions
  {
    public string LocalPath { get; init; } = "~/temp";
  }

  public record SendGridOptions : BaseEmailSenderOptions
  {
    public string SendGridApiKey { get; init; } = string.Empty;
  }
  public record SmtpOptions : BaseEmailSenderOptions
  {
    public string SmtpHost { get; init; } = string.Empty;
    public int SmtpPort { get; init; } = 0;
    public int SmtpSecureSocketEnum { get; init; } = 1; // 1 - Auto
    
    public string SmtpUsername { get; init; } = string.Empty;
    public string SmtpPassword { get; init; } = string.Empty;

    
  }
}
