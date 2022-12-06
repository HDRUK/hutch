using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using HutchManager.Data.Entities.Identity;

namespace HutchManager.Models.User;

public class UserModel
{
  [Required] [UsernameOrEmail] public string Username { get; set; } = string.Empty;

  [EmailAddress] public string? Email { get; set; } 
  
  public UserModel(ApplicationUser entity)
  {
    Username = entity.UserName;
    Email = entity.Email;
  }
  
  [JsonConstructor]
  public UserModel(){}
  
}

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
      // this allows for "@username" usernames
      var regex = new Regex(@"^@.+"); // regex value to check valid username i.e. starting with '@'
      return regex.IsMatch(usernameFormatToValidate); // return true if matched else false
    }
  
  }
  
