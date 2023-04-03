using System.Text.Json;
using System.Text.Json.Nodes;
using ROCrates.Converters;

namespace ROCrates.Models;

/// <summary>
/// Legacy workflow, added for completeness.
/// </summary>
public class Workflow : ComputationalWorkflow
{
  public Workflow(ROCrate? crate = null, string? identifier = null, JsonObject? properties = null,
    string? source = null,
    string? destPath = null, bool fetchRemote = false, bool validateUrl = false) : base(crate, identifier, properties,
    source, destPath, fetchRemote, validateUrl)
  {
    Types = new[] { "File", "SoftwareSourceCode", "Workflow" };
    SetProperty("@type", Types);
  }

  public Workflow()
  {
    Types = new[] { "File", "SoftwareSourceCode", "Workflow" };
    SetProperty("@type", Types);
  }

  /// <summary>
  /// Convert <see cref="Workflow"/> to JSON string.
  /// </summary>
  /// <returns>The <see cref="Workflow"/> as a JSON string.</returns>
  public override string Serialize()
  {
    var options = new JsonSerializerOptions
    {
      WriteIndented = true,
      Converters = { new WorkflowConverter() }
    };
    var serialised = JsonSerializer.Serialize(this, options);
    return serialised;
  }

  /// <summary>
  /// Create a <see cref="Workflow"/> from JSON properties.
  /// </summary>
  /// <param name="entityJson">The JSON representing the <see cref="Workflow"/></param>
  /// <param name="roCrate">The RO-Crate for the <see cref="Workflow"/></param>
  /// <returns>The deserialised <see cref="Workflow"/></returns>
  public new static Workflow? Deserialize(string entityJson, ROCrate roCrate)
  {
    var options = new JsonSerializerOptions
    {
      WriteIndented = true,
      Converters = { new WorkflowConverter() }
    };
    var deserialized = JsonSerializer.Deserialize<Workflow>(entityJson, options);
    if (deserialized is not null) deserialized.RoCrate = roCrate;
    return deserialized;
  }
}
