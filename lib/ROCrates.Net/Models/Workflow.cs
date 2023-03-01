using System.Text.Json.Nodes;

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
    Types = new []{ "File", "SoftwareSourceCode", "Workflow" };
    SetProperty("@type", Types);
  }
}
