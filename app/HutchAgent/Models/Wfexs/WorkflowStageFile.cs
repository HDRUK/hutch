using YamlDotNet.Serialization;

namespace HutchAgent.Models.Wfexs;

public class WorkflowStageFile
{
  [YamlMember(Alias = "workflow_id")] public string WorkflowId { get; set; } = string.Empty;

  [YamlMember(Alias = "workflow_config")]
  public Configuration WorkflowConfig { get; set; } = new();

  [YamlMember(Alias = "nickname")] public string Nickname { get; set; } = string.Empty;

  [YamlMember(Alias = "cacheDir")] public string CacheDirectory { get; set; } = "/tmp/wfexszn6siq2jtmpcache";

  [YamlMember(Alias = "crypt4gh")] public Crypt4ghOptions Crypt4Gh { get; set; } = new();

  [YamlMember(Alias = "outputs")] public object Outputs = new();

  [YamlMember(Alias = "params")] public object? Params { get; set; }
}

public class Configuration
{
  [YamlMember(Alias = "container")] public string Container { get; set; } = "podman";

  [YamlMember(Alias = "secure")] public bool Secure { get; set; } = false;

  // [YamlMember(Alias = "writable_containers")]
  // public bool WritableContainers { get; set; } = false;

  // [YamlMember(Alias = "cwl")] public CwlConfig Cwl { get; set; } = new();
  //
  // [YamlMember(Alias = "nextflow")] public NextFlowConfig Nextflow { get; set; } = new();
}

public class Crypt4ghOptions
{
  [YamlMember(Alias = "key")] public string Key { get; set; } = "cosifer_test1_cwl.wfex.stage.key";
  [YamlMember(Alias = "passphrase")] public string Passphrase { get; set; } = "mpel nite ified g";

  [YamlMember(Alias = "pub")] public string Public { get; set; } = "cosifer_test1_cwl.wfex.stage.pub";
}

// public class CwlConfig
// {
//   [YamlMember(Alias = "version")] public string Version { get; set; } = "3.1.20210628163208";
// }
//
// public class NextFlowConfig
// {
//   [YamlMember(Alias = "version")] public string Version { get; set; } = "19.04.1";
//
//   [YamlMember(Alias = "profile")] public string Profile { get; set; } = string.Empty;
// }
