using LinkLiteManager.Models.User;

namespace LinkLiteManager.Models.Account;

public record LoginResult
{
  public UserProfileModel? User { get; set; }
  public bool? IsUnconfirmedAccount { get; init; } = null;
  public List<string> Errors { get; init; } = new();
}
