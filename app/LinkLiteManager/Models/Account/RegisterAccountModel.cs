
using System.ComponentModel.DataAnnotations;

namespace LinkLiteManager.Models.Account;

public record RegisterAccountModel(
  [Required]
  [DataType(DataType.Text)]
  string FullName,

  [Required]
  [EmailAddress]
  string Email,

  [Required]
  [EmailAddress]
  string EmailConfirm,

  [Required]
  [DataType(DataType.Password)]
  string Password,

  [Required]
  [DataType(DataType.Password)]
  string PasswordConfirm
);

