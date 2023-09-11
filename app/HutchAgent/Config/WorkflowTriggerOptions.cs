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
}
