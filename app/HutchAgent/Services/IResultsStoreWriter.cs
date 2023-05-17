namespace HutchAgent.Services;

public interface IResultsStoreWriter
{
  public Task<bool> StoreExists();

  public Task WriteToStore(string resultPath);

  public Task<bool> ResultExists(string resultPath);
}
