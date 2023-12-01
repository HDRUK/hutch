namespace DummyControllerApi.Config;

public class WebhookOptions
{
  public string CallbackUrl { get; set; } = string.Empty;
  public bool VerifySsl { get; set; } = true;
  public List<string> Events { get; set; } = new();
}
