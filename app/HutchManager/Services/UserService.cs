using System.Globalization;
using System.Security.Claims;
using HutchManager.Auth;
using HutchManager.Config;
using HutchManager.Data;
using HutchManager.Data.Entities.Identity;
using HutchManager.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace HutchManager.Services;

public class UserService
{
  private readonly ApplicationDbContext _db;
  private readonly IUserClaimsPrincipalFactory<ApplicationUser> _principalFactory;
  private readonly RegistrationOptions _registerConfig;

  public UserService(
    ApplicationDbContext db,
    IOptions<RegistrationOptions> registerConfig,
    IUserClaimsPrincipalFactory<ApplicationUser> principalFactory)
  {
    _db = db;
    _principalFactory = principalFactory;
    _registerConfig = registerConfig.Value;
  }

  /// <summary>
  /// Checks if the provided Email Address is in the Registration Allowlist
  /// (or if the allowlist is disabled, simply returns true without hitting the db)
  /// </summary>
  /// <param name="email">The email address to check</param>
  /// <returns></returns>
  public async Task<bool> CanRegister(string email)
    => !_registerConfig.UseAllowList ||
        (await _db.RegistrationAllowlist.FindAsync(email) is not null);

  /// <summary>
  /// Build up a client profile for a user
  /// </summary>
  /// <param name="user"></param>
  /// <returns></returns>
  public async Task<UserProfileModel> BuildProfile(ApplicationUser user)
    => await BuildProfile(await _principalFactory.CreateAsync(user));

  public Task<UserProfileModel> BuildProfile(ClaimsPrincipal user)
  {
    // do a single-pass map of claims to a dictionary of those we care about
    var profileClaimTypes = new[] { ClaimTypes.Email, CustomClaimTypes.FullName, CustomClaimTypes.UICulture };
    var profileClaims = user.Claims.Aggregate(new Dictionary<string, string>(), (claims, claim) =>
    {
      if (profileClaimTypes.Contains(claim.Type))
      {
        // we only add the first claim of type
        if (!claims.ContainsKey(claim.Type))
        {
          claims[claim.Type] = claim.Value;
        }
      }
      return claims;
    });

    // construct a User Profile
    var profile = new UserProfileModel(
      profileClaims[ClaimTypes.Email],
      profileClaims[CustomClaimTypes.FullName],
      profileClaims.GetValueOrDefault(CustomClaimTypes.UICulture) ?? CultureInfo.CurrentCulture.Name
    );

    return Task.FromResult(profile);
  }

  /// <summary>
  /// Set a User's UI Culture
  /// </summary>
  /// <param name="userId"></param>
  /// <param name="cultureName"></param>
  /// <returns></returns>
  public async Task SetUICulture(string userId, string cultureName)
  {
    // verify it's a real culture name
    var culture = CultureInfo.GetCultureInfoByIetfLanguageTag(cultureName);

    var user = await _db.Users.FindAsync(userId);
    if (user is null) throw new KeyNotFoundException();

    user.UICulture = culture.Name;

    await _db.SaveChangesAsync();
  }
}
