using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace HutchManager.Models.Account;

public record LoginModel(
  [Required]
  [UsernameOrEmail]
  string Username,

  [Required]
  [DataType(DataType.Password)]
  string Password
);

/// <summary>
/// Custom validation attribute (UsernameOrEmailAttribute) to allow log in
/// for valid email and
/// username (DECSYS style "@admin" usernames)
/// </summary>
public class UsernameOrEmailAttribute : ValidationAttribute 
{
  protected override ValidationResult? IsValid(
    object? value, ValidationContext validationContext)
  {
    var stringValue = value?.ToString(); // string conversion
    var errorMsg = new ValidationResult(GetErrorMessage()); // error message

    if (stringValue is not null && // if not null
        (IsValidEmailAddress(stringValue) || IsValidUsernameFormat(stringValue))) // AND is either valid email or username
      return ValidationResult.Success; // returns null, which indicates success

    return errorMsg; // else return error
  }
  
  private static string GetErrorMessage()
  {
    return $"Valid email or username required"; // error message
  }
  private static bool IsValidEmailAddress(string emailToValidate)
  {
    var emailAttribute = new EmailAddressAttribute(); // Get email validation attribute instance
    return emailAttribute.IsValid(emailToValidate); // return true if valid email else false
  }

  private static bool IsValidUsernameFormat(string usernameFormatToValidate)
  {
    // this allows for DECSYS style "@admin" usernames
    var regex = new Regex(@"^@.+"); // regex value to check valid username i.e. starting with '@'
    return regex.IsMatch(usernameFormatToValidate); // return true if matched else false
  }
  
}
