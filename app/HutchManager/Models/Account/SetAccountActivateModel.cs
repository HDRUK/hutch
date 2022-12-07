using System.ComponentModel.DataAnnotations;

namespace HutchManager.Models.Account;

/// <summary>
/// Model for setting a known User's password
/// </summary>
/// <param name="FullName"></param>
/// <param name="Password"></param>
public record SetAccountActivateModel(
  [Required]
  string FullName,

  [Required]
  [DataType(DataType.Password)]
  string Password
);

/// <summary>
/// Model for setting a Password and FullName when the User isn't already known implicitly by the system
/// (i.e. they are unauthenticated, so Anonymous in that sense).
/// </summary>
/// <param name="Credentials">The Credentials that authorise the user account activation: UserId and Generate UserToken Token</param>
/// <param name="Data">The Payload for the user account activation: Password and FullName</param>
public record AnonymousSetAccountActivateModel(
  UserTokenModel Credentials,
  SetAccountActivateModel Data
);
