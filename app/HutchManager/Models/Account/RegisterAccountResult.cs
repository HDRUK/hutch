namespace HutchManager.Models.Account;

public record RegisterAccountResult
{
  public bool? IsExistingUser { get; init; } = null;
  public bool? IsNotAllowlisted { get; init; } = null;
  public List<string> Errors { get; init; } = new();
};


