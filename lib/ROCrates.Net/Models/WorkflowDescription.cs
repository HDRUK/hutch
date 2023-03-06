using System.Text.Json.Nodes;

namespace ROCrates.Models;

/// <summary>
/// Abstract CWL description of the main workflow.
/// </summary>
public class WorkflowDescription : ComputationalWorkflow
{
  public WorkflowDescription(ROCrate crate, string? identifier = null, JsonObject? properties = null,
    string? source = null, string? destPath = null, bool fetchRemote = false, bool validateUrl = false) : base(crate,
    identifier, properties, source, destPath, fetchRemote, validateUrl)
  {
    Types = new[] { "File", "SoftwareSourceCode", "HowTo" };
    SetProperty("@type", Types);
  }
}
