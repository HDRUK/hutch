using Microsoft.Extensions.Options;

namespace HutchAgent.Models;

public class WorkflowTriggerOptions
{
  public string ExecutorPath { get; set; } = string.Empty;

  public string VirtualEnvironmentPath { get; set; } = string.Empty;

  public string LocalConfigPath { get; set; } = string.Empty;

  public string StageFilePath { get; set; } = string.Empty;
}
