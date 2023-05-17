namespace HutchAgent.Services;

public interface IResultsStoreWriterAsync
{
  public Task<bool> StoreExists();

  public Task WriteToStore(string resultPath);

  public Task<bool> ResultExists(string resultPath);
}
