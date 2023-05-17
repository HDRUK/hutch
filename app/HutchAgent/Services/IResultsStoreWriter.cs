namespace HutchAgent.Services;

public interface IResultsStoreWriter
{
  public bool StoreExists();

  public void WriteToStore(string resultPath);

  public bool ResultExists(string resultPath);
}
