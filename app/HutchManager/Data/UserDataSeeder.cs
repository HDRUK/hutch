using System.Security.Claims;
using HutchManager.Data.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace HutchManager.Data;

public class UserDataSeeder
{
  
        const string _defaultAdminUsername = "admin";

        public static async Task Seed(
            UserManager<ApplicationUser> users,
            IPasswordHasher<ApplicationUser> passwords,
            IConfiguration config)
        {
            // Seed an initial super user to use for setup


            // prep username
            var configuredUsername = config["Root:Username"];
            var username = string.IsNullOrWhiteSpace(configuredUsername)
                ? _defaultAdminUsername
                : configuredUsername;
            username = $"@{username}"; // Prefix the username to show it's not an email


            // check an actual password has been configured
            var pwd = config["Root:Password"];
            if (string.IsNullOrEmpty(pwd))
            {
                throw new ApplicationException(@"
A non-empty password must be configured for seeding the initial Admin User.
Please set Root:Password in a settings or user secrets file,
or the environment variable DOTNET_Hosted_AdminPassword");
            }

            // Add the user if they don't exist, else update them,
            var email = config["Root:EmailAddress"] ?? "admin@local"; //use 'admin@local' as email if Root:EmailAddress id not configured
            var superAdmin = await users.FindByEmailAsync(email);
            if (superAdmin is null)
            {
                var user = new ApplicationUser
                {
                    UserName = username,
                    FullName = "Super Admin",
                    Email = email,
                    EmailConfirmed = true
                };

                user.PasswordHash = passwords.HashPassword(user, pwd);

                await users.CreateAsync(user);
                await users.AddClaimAsync(user,
                    new Claim(ClaimTypes.Role, "local.admin"));
            } else
            {
                // update username / password
                superAdmin.UserName = username;
                superAdmin.PasswordHash = passwords.HashPassword(superAdmin, pwd);
                await users.UpdateAsync(superAdmin);
            }
        }
}
