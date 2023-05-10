using System.Linq.Expressions;

namespace HutchAgent.Tests.AsyncHandlers;

public class AsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
  public AsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable)
  {
  }

  public AsyncEnumerable(Expression expression) : base(expression)
  {
  }

  public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken())
  {
    return new AsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
  }

  IQueryProvider IQueryable.Provider => new AsyncQueryProvider<T>(this);
}
