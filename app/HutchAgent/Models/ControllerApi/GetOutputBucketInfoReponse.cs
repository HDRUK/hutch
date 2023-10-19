namespace HutchAgent.Models.ControllerApi;

public class GetOutputBucketInfoReponse : FileStorageDetails
{
  /// <summary>
  /// Hutch doesn't use this, but the API sends it
  /// </summary>
  public string SubId { get; set; } = string.Empty;
}
