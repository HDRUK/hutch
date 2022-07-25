
using System.ComponentModel.DataAnnotations;

namespace HutchManager.Models.Account;

public record RegisterAccountModel(
  [Required]
  [DataType(DataType.Text)]
  string FullName,

  [Required]
  [EmailAddress]
  string Email,

  [Required]
  [DataType(DataType.Password)]
  string Password
);

