namespace DummyControllerApi.Services;

public class InMemoryApprovalQueue
{
  private readonly Queue<string> _queue = new();

  public void Enqueue(string subId)
  {
    _queue.Enqueue(subId);
  }

  public string Dequeue()
  {
    return _queue.Dequeue();
  }
}
