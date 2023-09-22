namespace HutchAgent.Models;

/// <summary>
/// Public model for reporting a Job's Status in API responses.
/// </summary>
public class JobStatusModel
{
  /// <summary>
  /// The Id of the Job
  /// </summary>
  public required string Id { get; set; }
  
  /// <summary>
  /// The current status of the Job
  /// </summary>
  public required string Status { get; set; }
}
