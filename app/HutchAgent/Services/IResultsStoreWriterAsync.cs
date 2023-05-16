namespace HutchAgent.Services;

public interface IResultsStoreWriterAsync
{
  public Task<bool> StoreExists(string location);

  public Task WriteToStore(string resultPath);

  public Task<bool> ResultExists(string resultPath);
}
