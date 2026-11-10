using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using SystemX.Application.Abstractions.Persistence;
using SystemX.Infrastructure.Persistence.EfCore.Context;

namespace SystemX.Infrastructure.Persistence.EfCore.Transactions;

/// <summary>
/// Provides read database connections backed by the underlying <see cref="SystemXDbContext"/>.
/// </summary>
public sealed class EfReadConnectionFactory : IReadConnectionFactory
{
    private readonly SystemXDbContext _context;

    /// <summary>
    /// Initializes a new instance of <see cref="EfReadConnectionFactory"/>.
    /// </summary>
    /// <param name="context">The underlying DbContext.</param>
    public EfReadConnectionFactory(SystemXDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Gets and opens the underlying database connection.
    /// The connection belongs to the DbContext and callers must NOT dispose it.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The opened database connection.</returns>
    public async Task<DbConnection> GetOpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = _context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await _context.Database.OpenConnectionAsync(cancellationToken);
        }

        return connection;
    }
}
