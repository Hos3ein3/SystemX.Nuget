using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using SystemX.Application.Abstractions.Events;
using SystemX.Application.Abstractions.MultiTenancy;
using SystemX.Application.Abstractions.Persistence;
using SystemX.Application.Abstractions.Security;
using SystemX.Infrastructure.Persistence.EfCore.Configuration;
using SystemX.Infrastructure.Persistence.EfCore.Connections;
using SystemX.Infrastructure.Persistence.EfCore.Context;
using SystemX.SharedKernel.MultiTenancy;
using SystemX.SharedKernel.Time;

namespace SystemX.Infrastructure.Persistence.EfCore.Sessions;

internal sealed class DatabaseFactory<TContext> : IDatabaseFactory
    where TContext : SystemXDbContext
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly SystemXEfCoreOptions _options;
    private readonly ITenantContext? _ambientTenantContext;

    public DatabaseFactory(
        IServiceScopeFactory scopeFactory,
        SystemXEfCoreOptions options,
        ITenantContext? ambientTenantContext = null)
    {
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _ambientTenantContext = ambientTenantContext;
    }

    public Task<IDatabaseSession> CreateSessionAsync(CancellationToken cancellationToken = default)
    {
        var tenantContext = _ambientTenantContext is null ? null : new FixedTenantContext(_ambientTenantContext.Tenant);
        return Task.FromResult<IDatabaseSession>(Create(tenantContext, _options));
    }

    public Task<IDatabaseSession> CreateSessionForTenantAsync(TenantInfo tenant, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(tenant);
        return Task.FromResult<IDatabaseSession>(Create(new FixedTenantContext(tenant), _options));
    }

    public Task<IDatabaseSession> CreateHostSessionAsync(CancellationToken cancellationToken = default)
    {
        var hostOptions = _options.Clone();
        hostOptions.IgnoreTenancy = true;
        return Task.FromResult<IDatabaseSession>(Create(null, hostOptions));
    }

    private EfDatabaseSession Create(FixedTenantContext? tenantContext, SystemXEfCoreOptions options)
    {
        var scope = _scopeFactory.CreateAsyncScope();
        try
        {
            var sp = scope.ServiceProvider;

            var connectionString = ConnectionStringResolver.Resolve(
                options,
                sp.GetService<ITenantConnectionResolver>(),
                tenantContext?.Tenant,
                ConnectionStringResolver.DefaultName);

            var builder = new DbContextOptionsBuilder<TContext>();
            options.ConfigureProvider!(builder, connectionString);
            builder.ReplaceService<IModelCacheKeyFactory, SystemXModelCacheKeyFactory>();
            options.ConfigureDbContext?.Invoke(sp, builder);

            var dependencies = new SystemXDbContextDependencies<TContext>(
                options,
                sp.GetRequiredService<IClock>(),
                sp.GetRequiredService<IDomainEventDispatcher>(),
                tenantContext,
                sp.GetService<ICurrentUser>(),
                sp.GetService<ITenantSchemaProvider>());

            var context = ActivatorUtilities.CreateInstance<TContext>(sp, builder.Options, dependencies);
            return new EfDatabaseSession(scope, context, tenantContext?.Tenant);
        }
        catch
        {
            scope.Dispose();
            throw;
        }
    }
}
