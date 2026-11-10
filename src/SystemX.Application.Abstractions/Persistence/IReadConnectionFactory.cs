using System.Data.Common;

namespace SystemX.Application.Abstractions.Persistence;

/// <summary>
/// Provides access to the underlying database connection for low-level or high-performance read queries (e.g., Dapper).
/// </summary>
/// <remarks>
/// The connection returned by this factory is managed by the ambient database context or session. Callers must NOT dispose it.
/// </remarks>
public interface IReadConnectionFactory
{
    /// <summary>
    /// Gets or opens an active database connection for read operations.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The open database connection.</returns>
    Task<DbConnection> GetOpenConnectionAsync(CancellationToken cancellationToken = default);
}
