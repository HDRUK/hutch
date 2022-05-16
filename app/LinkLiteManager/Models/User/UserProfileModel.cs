namespace LinkLiteManager.Models.User;

public record BaseUserProfileModel(
  string Email,
  string FullName,
  string UICulture
);

public record UserProfileModel(
  string Email,
  string FullName,
  string UICulture
)
  : BaseUserProfileModel(
      Email,
      FullName,
      UICulture);

