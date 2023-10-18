using System.ComponentModel.DataAnnotations;

namespace DummyControllerApi.Models;

public class FilesReadyRequestModel
{
  [Required]
  public string SubId { get; set; } = string.Empty;
  
  public List<string> Files { get; set; } = new();
}
