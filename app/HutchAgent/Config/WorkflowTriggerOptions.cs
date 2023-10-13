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
  /// <para>
  /// If this is a non-empty value, Workflow Execution will be skipped,
  /// and the file path provided in this value will be used as the output from execution.
  /// </para>
  /// <para>
  /// For example, `/path/to/execution.crate.zip` would cause Hutch to not execute the job's workflow,
  /// but instead queue an InitiateEgress action to use the zip file in the path as if it was the execution output.
  /// Note that a zip file is expected, (ideally an RO-Crate for authenticity).
  /// </para>
  /// <para>
  /// Intended for development and testing when actual workflow execution is not needed or desirable,
  /// but instead the post-execution behaviours can be tested with a static output.
  /// </para>
  /// <para>
  /// Relative paths are relative to Hutch's working directory root
  /// (<see cref="PathOptions.WorkingDirectoryBase"/>) - not a specific job's.
  /// </para>
  /// 
  /// </summary>
  public string SkipExecutionUsingOutputFile { get; set; } = string.Empty;
  
  /// <summary>
  /// Don't ask WfExS for a full provenance output crate (i.e. don't use `--full`).
  /// `--full` is typically preferred but can be unreliable in some environments,
  /// so it can be turned off here.
  /// </summary>
  public bool SkipFullProvenanceCrate { get; set; }

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
