namespace DummyControllerApi.Models;

public class HutchApprovalRequestModel
{
  public string Status { get; set; } = "FullyApproved";
  public Dictionary<string, bool> Files { get; set; } = new();
}
