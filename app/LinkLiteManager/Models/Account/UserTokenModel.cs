using System.ComponentModel.DataAnnotations;

namespace LinkLiteManager.Models.Account;

public record UserTokenModel(
    [Required]
    string UserId,
    [Required]
    string Token);

