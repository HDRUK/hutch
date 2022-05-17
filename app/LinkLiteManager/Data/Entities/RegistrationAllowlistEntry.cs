using System.ComponentModel.DataAnnotations;

namespace LinkLiteManager.Data.Entities;

public class RegistrationAllowlistEntry
{
  [Key] // TODO: partial email allow listing e.g. nottingham.ac.uk
  public string EmailAddress { get; set; } = string.Empty;
}
