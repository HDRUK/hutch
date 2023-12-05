namespace DummyControllerApi.Config;

public class WebHookOptions
{
  public string CallbackUrl { get; set; } = string.Empty;
  public bool VerifySsl { get; set; } = true;
  public List<string> Events { get; set; } = new();
}
