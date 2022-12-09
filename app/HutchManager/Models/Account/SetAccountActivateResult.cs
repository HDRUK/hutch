using HutchManager.Models.User;

namespace HutchManager.Models.Account;
public record SetAccountActivateResult
{
  public UserProfileModel? User { get; init; } = null;
  public bool? IsAccountConfirmed { get; init; } = null;
  public List<string>? Errors { get; init; } = new();
}
