using HutchAgent.Services.Contracts;

namespace HutchAgent.Services.ActionHandlers;

public class FetchAndExecuteActionHandler : IActionHandler
{
  private readonly ExecuteActionHandler _executeHandler;

  public FetchAndExecuteActionHandler(ExecuteActionHandler executeHandler)
  {
    _executeHandler = executeHandler;
  }

  public async Task HandleAction(string jobId)
  {
    // Fetch
    
    // Execute
    await _executeHandler.HandleAction(jobId);
  }
}
