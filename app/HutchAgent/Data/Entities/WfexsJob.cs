namespace HutchAgent.Data.Entities;

/// <summary>
/// Represents a run of WfExS. Contains the path to where the RO-Crate should be unpacked, the ID of the WfExS run
/// and whether or not the WfExS run has been completed.
/// </summary>
public class WfexsJob
{
  public int Id { get; set; }
  public string UnpackedPath { get; set; } = string.Empty;
  public string WfexsRunId { get; set; } = string.Empty;
  public bool RunFinished { get; set; }
}
