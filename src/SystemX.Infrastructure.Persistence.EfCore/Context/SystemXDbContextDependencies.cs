using SystemX.Application.Abstractions.Events;
using SystemX.Application.Abstractions.MultiTenancy;
using SystemX.Application.Abstractions.Security;
using SystemX.Infrastructure.Persistence.EfCore.Configuration;
using SystemX.SharedKernel.Time;

namespace SystemX.Infrastructure.Persistence.EfCore.Context;

/// <summary>
/// Aggregates runtime dependencies required by <see cref="SystemXDbContext"/>.
/// </summary>
public class SystemXDbContextDependencies
{
    /// <summary>
    /// Initializes a new instance of <see cref="SystemXDbContextDependencies"/>.
    /// </summary>
    public SystemXDbContextDependencies(
        SystemXEfCoreOptions options,
        IClock clock,
        IDomainEventDispatcher domainEventDispatcher,
        ITenantContext? tenantContext = null,
        ICurrentUser? currentUser = null,
        ITenantSchemaProvider? schemaProvider = null)
    {
        Options = options ?? throw new ArgumentNullException(nameof(options));
        Clock = clock ?? throw new ArgumentNullException(nameof(clock));
        DomainEventDispatcher = domainEventDispatcher ?? throw new ArgumentNullException(nameof(domainEventDispatcher));
        TenantContext = tenantContext;
        CurrentUser = currentUser;
        SchemaProvider = schemaProvider;
    }

    /// <summary>Gets the EF Core options.</summary>
    public SystemXEfCoreOptions Options { get; }

    /// <summary>Gets the clock service.</summary>
    public IClock Clock { get; }

    /// <summary>Gets the domain event dispatcher.</summary>
    public IDomainEventDispatcher DomainEventDispatcher { get; }

    /// <summary>Gets the ambient tenant context, if available.</summary>
    public ITenantContext? TenantContext { get; }

    /// <summary>Gets the current user context, if available.</summary>
    public ICurrentUser? CurrentUser { get; }

    /// <summary>Gets the tenant schema provider, if available.</summary>
    public ITenantSchemaProvider? SchemaProvider { get; }
}

/// <summary>
/// Generic variant of <see cref="SystemXDbContextDependencies"/> typed to a specific context type.
/// </summary>
/// <typeparam name="TContext">The context type.</typeparam>
public sealed class SystemXDbContextDependencies<TContext> : SystemXDbContextDependencies
    where TContext : SystemXDbContext
{
    /// <summary>
    /// Initializes a new instance of <see cref="SystemXDbContextDependencies{TContext}"/>.
    /// </summary>
    public SystemXDbContextDependencies(
        SystemXEfCoreOptions options,
        IClock clock,
        IDomainEventDispatcher domainEventDispatcher,
        ITenantContext? tenantContext = null,
        ICurrentUser? currentUser = null,
        ITenantSchemaProvider? schemaProvider = null)
        : base(options, clock, domainEventDispatcher, tenantContext, currentUser, schemaProvider)
    {
    }
}
