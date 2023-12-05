using System.ComponentModel.DataAnnotations;

namespace DummyControllerApi.Models;

public class FinalOutcomeRequestModel
{
  [Required] public string SubId { get; set; } = string.Empty;

  [Required] public string File { get; set; } = string.Empty;
}
