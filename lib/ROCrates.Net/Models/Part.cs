using System.Text.Json.Serialization;

namespace ROCrates.Models;

/// <summary>
/// Represents an "ID tag" linking a property in one object in an RO-Crate to another object in the same crate.
/// </summary>
public class Part
{
  [JsonPropertyName("@id")] public string Id { get; set; } = string.Empty;
}
