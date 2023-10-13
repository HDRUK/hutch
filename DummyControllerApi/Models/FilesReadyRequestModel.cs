namespace DummyControllerApi.Models;

public class FilesReadyRequestModel
{
  public string SubId { get; set; } = string.Empty;
  public List<string> Files { get; set; } = new();
}
