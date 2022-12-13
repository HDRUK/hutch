using System.Globalization;
using System.Security.Claims;
using HutchManager.Auth;
using HutchManager.Constants;
using HutchManager.Data;
using HutchManager.Data.Entities.Identity;
using HutchManager.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;
namespace HutchManager.Services;

public class UserService
{
  private readonly ApplicationDbContext _db;
  private readonly IUserClaimsPrincipalFactory<ApplicationUser> _principalFactory;
  private readonly UserManager<ApplicationUser> _users;
  private readonly IConfiguration _config;
  public UserService(
    ApplicationDbContext db,
    IUserClaimsPrincipalFactory<ApplicationUser> principalFactory,
    IConfiguration config,
    UserManager<ApplicationUser> users)
  {
    _db = db;
    _principalFactory = principalFactory;
    _users = users;
    _config = config;
  }

  /// <summary>
  /// Checks if the provided Email Address is in the Registration Allowlist
  /// (or if the allowlist is disabled, simply returns true without hitting the db)
  /// </summary>
  /// <param name="email">The email address to check</param>
  /// <returns></returns>
  public async Task<bool> CanRegister(string email)
  {
    var regOptions = new RegistrationOptions();
    _config.GetSection(RegistrationOptions.UserAccounts).Bind(regOptions);
    return regOptions.Registration == "free" ||
           (await _db.RegistrationAllowlist.FindAsync(email) is not null);
  }
  /// <summary>
  /// Checks if User Registration is disabled
  /// </summary>
  /// <returns></returns>
  public bool IsDisabled()
  {
    var regOptions = new RegistrationOptions();
    _config.GetSection(RegistrationOptions.UserAccounts).Bind(regOptions);
    return regOptions.Registration == "disabled";
  }
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

  /// <summary>
  /// Create User given a username
  /// </summary>
  /// <param name="userModel"></param>
  public async Task Create(UserModel userModel)
  {
    // Autogenerate email address for @username users
    if (userModel.Username.StartsWith("@"))
    {
      userModel.Email = userModel.Username.Trim('@') + "@local";
    }
    else
    {
      userModel.Email = userModel.Username;
    }
    
    var user = new ApplicationUser()
    {
      UserName = userModel.Username,
      Email = userModel.Email
    };
    await _users.CreateAsync(user);
  }
  
  /// <summary>
  /// List all Users
  /// </summary>
  /// <returns></returns>
  public async Task<List<UserModel>> List()
  {
    var list = await _db.Users
      .AsNoTracking()
      .ToListAsync();
    return list.ConvertAll<UserModel>(x => new(x));
  }
  
  /// <summary>
  /// Delete User by ID
  /// </summary>
  /// <param name="userId"></param>
  /// <exception cref="KeyNotFoundException"></exception>
  public async Task Delete(string userId)
  {
    var entity = await _db.Users
      .AsNoTracking()
      .FirstOrDefaultAsync(x => x.Id == userId);
    if (entity is null)
      throw new KeyNotFoundException(
        $"No User with ID: {userId}");
    _db.Users.Remove(entity);
    await _db.SaveChangesAsync();
  }
}
