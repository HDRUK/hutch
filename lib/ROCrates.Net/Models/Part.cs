namespace ROCrates.Models;

/// <summary>
/// Represents an "ID tag" linking a property in one object in an RO-Crate to another object in the same crate.
/// </summary>
public class Part
{
  public const string Key = "@id";
  public string Identifier { get; set; } = string.Empty;
}
