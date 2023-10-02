using YamlDotNet.Serialization;

namespace HutchAgent.Models.Wfexs;

/// <summary>
/// Represents properties we care about in Wfexs' Local Config YAML
/// </summary>
public class WfexsLocalConfig
{
  /// <summary>
  /// Path to Wfexs' Working Directory for workflow executions
  /// </summary>
  public string WorkDir { get; set; } = null!;
}
