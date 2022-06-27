using System.ComponentModel.DataAnnotations;

namespace HutchManager.Models.Account;

public record LoginModel(
  [Required]
  [EmailAddress]
  string Username,

  [Required]
  [DataType(DataType.Password)]
  string Password
);
