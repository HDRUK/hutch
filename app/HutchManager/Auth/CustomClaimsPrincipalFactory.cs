using System.Security.Claims;
using HutchManager.Data.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace HutchManager.Auth;

public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
{
  public CustomClaimsPrincipalFactory(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IOptions<IdentityOptions> optionsAccessor)
    : base(userManager, roleManager, optionsAccessor) { }

  public async override Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
  {
    var principal = await base.CreateAsync(user);
    var identity = (ClaimsIdentity?)principal.Identity
      ?? throw new InvalidOperationException(
        "No ClaimsIdentity present on this user");

    List<Claim> claims = new()
    {
      new Claim(CustomClaimTypes.FullName, user.FullName),
      new Claim(CustomClaimTypes.UICulture, user.UICulture)
    };

    identity.AddClaims(claims);
    return principal;
  }
}

