namespace HutchManager.Models.Account;

public record UserActivationTokenModel
{
  public string ActivationLink { get; set; } = string.Empty;
}

