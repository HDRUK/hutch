namespace HutchAgent.Services;

public interface IResultsStoreWriter
{
  public bool StoreExists(string location);

  public void WriteToStore(string resultPath);

  public bool ResultExists(string resultPath);
}
