namespace HutchAgent.Models.ControllerApi;

public class FilesReadyForReviewRequest
{
  public List<string> Files { get; set; } = new();
}
