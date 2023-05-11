using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace HutchAgent.Tests.AsyncHandlers;

public class AsyncQueryProvider<TEntity> : IAsyncQueryProvider
{
  private readonly IQueryProvider _inner;

  public AsyncQueryProvider(IQueryProvider inner)
  {
    _inner = inner;
  }

  public IQueryable CreateQuery(Expression expression)
  {
    return new AsyncEnumerable<TEntity>(expression);
  }

  public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
  {
    return new AsyncEnumerable<TElement>(expression);
  }

  public object? Execute(Expression expression)
  {
    return _inner.Execute(expression);
  }

  public TResult Execute<TResult>(Expression expression)
  {
    return _inner.Execute<TResult>(expression);
  }

  public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = new())
  {
    return _inner.Execute<TResult>(expression);
  }
}
