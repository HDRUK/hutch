namespace HutchAgent.Tests.AsyncHandlers;

public class AsyncEnumerator<T> : IAsyncEnumerator<T>
{
  private readonly IEnumerator<T> _inner;

  public AsyncEnumerator(IEnumerator<T> inner)
  {
    _inner = inner;
  }

  public ValueTask<bool> MoveNextAsync()
  {
    return ValueTask.FromResult(_inner.MoveNext());
  }

  public T Current => _inner.Current;

  public ValueTask DisposeAsync()
  {
    _inner.Dispose();
    return ValueTask.CompletedTask;
  }
}
