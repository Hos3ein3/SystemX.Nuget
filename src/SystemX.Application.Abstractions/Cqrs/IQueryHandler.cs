namespace SystemX.Application.Abstractions.Cqrs;

/// <summary>
/// Defines a handler for a query.
/// </summary>
/// <typeparam name="TQuery">The query type.</typeparam>
/// <typeparam name="TResult">The query result type.</typeparam>
public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
{
    /// <summary>
    /// Handles the query.
    /// </summary>
    /// <param name="query">The query instance.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The query execution result.</returns>
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
