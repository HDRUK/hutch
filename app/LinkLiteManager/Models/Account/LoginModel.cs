using System.ComponentModel.DataAnnotations;

namespace LinkLiteManager.Models.Account;

public record LoginModel(
  [Required]
  [EmailAddress]
  string Username,

  [Required]
  [DataType(DataType.Password)]
  string Password
);
