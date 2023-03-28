namespace HutchAgent.Models;

public class WorkflowTriggerOptions
{
  public const string Type = "WfExS";

  public const string ExecutorPath = "/home/vasiliki/WfExS-backend/";

  public const string VirtualEnvironmentPath = ".pyWEenv/bin/activate";

  public const string LocalConfigPath = "workflow_examples/local_config.yaml";
  
  public const string StageFilePath = "workflow_examples/ipc/hutch_cwl.wfex.stage";

}
