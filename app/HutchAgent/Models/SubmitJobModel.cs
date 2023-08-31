namespace HutchAgent.Models;

public class SubmitJobModel
{
  public string JobId { get; set; } = string.Empty;

  public IFormFile Crate { get; set; } = null!;
}
