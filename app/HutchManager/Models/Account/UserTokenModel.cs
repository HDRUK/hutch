using System.ComponentModel.DataAnnotations;

namespace HutchManager.Models.Account;

public record UserTokenModel(
    [Required]
    string UserId,
    [Required]
    string Token);

