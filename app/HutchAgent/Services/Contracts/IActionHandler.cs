namespace HutchAgent.Services.Contracts;

public interface IActionHandler
{
  public Task HandleAction(string jobId);
}
