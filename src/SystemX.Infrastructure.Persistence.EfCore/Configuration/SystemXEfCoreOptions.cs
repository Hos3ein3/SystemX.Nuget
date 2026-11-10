using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using SystemX.SharedKernel.MultiTenancy;

namespace SystemX.Infrastructure.Persistence.EfCore.Configuration;

/// <summary>
/// Configuration options for SystemX Entity Framework Core persistence.
/// </summary>
public sealed class SystemXEfCoreOptions
{
    /// <summary>
    /// Gets or sets the connection string of the host (shared) database.
    /// Used for Row and Schema multi-tenancy models and when no tenant is active.
    /// </summary>
    public string? HostConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the delegate that configures the database provider on <see cref="DbContextOptionsBuilder"/>.
    /// Provider packages (e.g., SqlServer, Postgresql) set this.
    /// </summary>
    public Action<DbContextOptionsBuilder, string>? ConfigureProvider { get; set; }

    /// <summary>
    /// Gets or sets the schema for shared entities (<see cref="ISharedEntity"/>) when the Schema tenancy model is used.
    /// Required when using the Schema tenancy model.
    /// </summary>
    public string? HostSchema { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a resolved tenant is required for tenant data access.
    /// When <see langword="true"/> (default), a missing tenant blocks access to tenant-scoped data.
    /// When <see langword="false"/>, no tenant runs in host mode without tenant filters.
    /// </summary>
    public bool RequireTenant { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the context completely ignores multi-tenancy.
    /// Set to <see langword="true"/> for host-only or administrative contexts.
    /// </summary>
    public bool IgnoreTenancy { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of recursive dispatch rounds for domain events raised by event handlers.
    /// Defaults to 5.
    /// </summary>
    public int MaxDomainEventDepth { get; set; } = 5;

    /// <summary>
    /// Gets or sets a value indicating whether default repositories, unit of work, and connection factories
    /// should be registered in the service container for this context. Defaults to <see langword="true"/>.
    /// </summary>
    public bool RegisterRepositories { get; set; } = true;

    /// <summary>
    /// Gets or sets the factory delegate to create a closed <see cref="DbConnection"/> for a given connection string.
    /// Provider packages (e.g., SqlServer, Postgresql) set this delegate.
    /// </summary>
    public Func<string, DbConnection>? CreateConnection { get; set; }

    /// <summary>
    /// Gets the dictionary of additional named connection strings for the host (e.g., "Reporting", "Audit").
    /// </summary>
    public IDictionary<string, string> NamedConnectionStrings { get; } =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets or sets an optional hook to further configure <see cref="DbContextOptionsBuilder"/> (e.g., interceptors, logging).
    /// Executed for request scopes and isolated sessions.
    /// </summary>
    public Action<IServiceProvider, DbContextOptionsBuilder>? ConfigureDbContext { get; set; }

    internal SystemXEfCoreOptions Clone()
    {
        var clone = (SystemXEfCoreOptions)MemberwiseClone();
        foreach (var kvp in NamedConnectionStrings)
        {
            clone.NamedConnectionStrings[kvp.Key] = kvp.Value;
        }

        return clone;
    }

    internal void Validate()
    {
        if (ConfigureProvider is null)
        {
            throw new InvalidOperationException(
                $"{nameof(ConfigureProvider)} must be configured on {nameof(SystemXEfCoreOptions)}.");
        }

        if (MaxDomainEventDepth < 1)
        {
            throw new InvalidOperationException(
                $"{nameof(MaxDomainEventDepth)} must be at least 1.");
        }
    }
}
