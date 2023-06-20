using YamlDotNet.Serialization;

namespace HutchAgent.Models;

public class WfexsStageFile
{
  [YamlMember(Alias="workflow_id")]

  public string WorkflowId { get; set; } = string.Empty;
  
  [YamlMember(Alias="workflow_config")]
  public Configuration WorkflowConfig { get; set; } = new Configuration();

  [YamlMember(Alias="nickname")]

  public string Nickname { get; set; } = string.Empty;
  
  [YamlMember(Alias="cacheDir")]

  public string CacheDirectory { get; set; } = string.Empty;

  [YamlMember(Alias="crypt4gh")]
  public Crypt4ghOptions Crypt4Gh { get; set; } = new();

  [YamlMember(Alias="outputs")]
  public Dictionary<string, Dictionary<string, string>> Outputs = new();

  [YamlMember(Alias = "params")] public object? Params { get; set; }

}

public class Configuration
{
  [YamlMember(Alias = "container")]
  public string Container { get; set; } = "docker";
  
  [YamlMember(Alias = "secure")]
  public bool Secure { get; set; } = false;
}

public class Crypt4ghOptions
{
  [YamlMember(Alias = "key")]

  public string Key { get; set; } = "cosifer_test1_cwl.wfex.stage.key";
  [YamlMember(Alias = "passphrase")]

  public string Passphrase { get; set; } = "mpel nite ified g";

  [YamlMember(Alias = "pub")] public string Public { get; set; } = "cosifer_test1_cwl.wfex.stage.pub";
}
