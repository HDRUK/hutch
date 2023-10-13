namespace HutchAgent.Models.ControllerApi;

public class FilesReadyForReviewRequest
{
  public string SubId { get; set; } = string.Empty;
  public List<string> Files { get; set; } = new();
}
