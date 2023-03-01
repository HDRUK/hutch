namespace ROCrates.Models;

/// <summary>
/// Represents an "ID tag" linking a property to an object one object in an RO-Crate to another.
/// </summary>
public class Part
{
  public const string Key = "@id";
  public string Identifier { get; set; } = string.Empty;
}
