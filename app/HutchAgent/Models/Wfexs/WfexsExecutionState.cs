using YamlDotNet.Serialization;

namespace HutchAgent.Models.Wfexs;

/// <summary>
/// Represents the properties Hutch cares about in Wfexs' `execution-state.yml` for a Workflow run.
/// </summary>
public class WfexsExecutionState
{
  [YamlMember(Alias = "exitVal")]
  public int ExitCode { get; set; }
  
  [YamlMember(Alias = "started")]
  public DateTimeOffset StartTime { get; set; }
  
  [YamlMember(Alias = "ended")]
  public DateTimeOffset EndTime { get; set; }
}
