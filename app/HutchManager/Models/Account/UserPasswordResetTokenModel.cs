namespace HutchManager.Models.Account;

public record UserPasswordResetTokenModel
{
  public string PasswordResetLink { get; set; } = string.Empty;
}

