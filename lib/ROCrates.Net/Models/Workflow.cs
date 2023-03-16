using System.Text.Json;
using System.Text.Json.Nodes;
using ROCrates.Converters;

namespace ROCrates.Models;

/// <summary>
/// Legacy workflow, added for completeness.
/// </summary>
public class Workflow : ComputationalWorkflow
{
  public Workflow(ROCrate crate, string? identifier = null, JsonObject? properties = null, string? source = null,
    string? destPath = null, bool fetchRemote = false, bool validateUrl = false) : base(crate, identifier, properties,
    source, destPath, fetchRemote, validateUrl)
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
}
