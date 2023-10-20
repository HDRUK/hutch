using HutchAgent.Constants;

namespace HutchAgent.Models;

public class ApprovalResult
{
  public ApprovalType Status { get; set; }

  public Dictionary<string, bool> FileResults { get; set; } = new();
}
