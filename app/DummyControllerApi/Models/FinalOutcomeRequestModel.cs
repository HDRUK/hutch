using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DummyControllerApi.Config;

namespace DummyControllerApi.Models;

public class FinalOutcomeRequestModel
{
  [Required] public string SubId { get; set; } = string.Empty;

  [Required] public string File { get; set; } = string.Empty;

  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public MinioOptions? BucketOptions { get; set; }
}
