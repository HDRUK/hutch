namespace HutchAgent.Results;

public class BasicResult
{
  /// <summary>
  /// Was the result successful?
  /// </summary>
  public bool IsSuccess { get; set; }

  /// <summary>
  /// Any error feedback
  /// </summary>
  public List<string> Errors { get; set; } = new();
}
