namespace HutchAgent.Config;

public class WorkflowTriggerOptions
{
  /// <summary>
  /// Path where Wfexs is installed
  /// </summary>
  public string ExecutorPath { get; set; } = string.Empty;

  /// <summary>
  /// Path to the Wfexs virtual environment
  /// </summary>
  public string VirtualEnvironmentPath { get; set; } = string.Empty;

  /// <summary>
  /// Path to the Wfexs local config file 
  /// </summary>
  public string LocalConfigPath { get; set; } = string.Empty;

  /// <summary>
  /// Should container images downloaded for workflows be included in the outputs?
  /// </summary>
  public bool IncludeContainersInOutput { get; set; }
  
  /// <summary>
  /// Ask WfExS for a full provenance output crarte (using `--full`).
  /// This is typically preferred but can be unreliable in some environments.
  /// </summary>
  public bool GenerateFullProvenanceCrate { get; set; }

  /// <summary>
  /// The container engine generated stage files should use e.g. `docker` (default), singularity or `podman`.
  /// Should match the `containerType` configured in the Executor's local config.
  /// </summary>
  // TODO enum this
  public string ContainerEngine { get; set; } = "docker";
  
  /// <summary>
  /// Keep Hutch attached to Executor processes that it triggers.
  /// This uses up threads in the pool, but is useful for debugging.
  /// </summary>
  public bool RemainAttached { get; set; }
}
