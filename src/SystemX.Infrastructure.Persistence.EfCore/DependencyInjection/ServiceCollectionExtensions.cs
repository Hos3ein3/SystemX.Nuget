using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SystemX.Application.Abstractions.Events;
using SystemX.Application.Abstractions.MultiTenancy;
using SystemX.Application.Abstractions.Persistence;
using SystemX.Application.Abstractions.Security;
using SystemX.Infrastructure.Persistence.EfCore.Configuration;
using SystemX.Infrastructure.Persistence.EfCore.Connections;
using SystemX.Infrastructure.Persistence.EfCore.Context;
using SystemX.Infrastructure.Persistence.EfCore.Repositories;
using SystemX.Infrastructure.Persistence.EfCore.Sessions;
using SystemX.Infrastructure.Persistence.EfCore.Transactions;
using SystemX.SharedKernel.Time;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Dependency injection extensions for registering SystemX EF Core persistence services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers a <see cref="SystemXDbContext"/> implementation with options, repositories, and factories.
    /// </summary>
    /// <typeparam name="TContext">The context type deriving from <see cref="SystemXDbContext"/>.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Configuration delegate for EF Core options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSystemXDbContext<TContext>(
        this IServiceCollection services,
        Action<SystemXEfCoreOptions> configure)
        where TContext : SystemXDbContext
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        var options = new SystemXEfCoreOptions();
        configure(options);
        options.Validate();

        services.TryAddSingleton(options);
        services.TryAddSingleton<IClock, SystemClock>();
        services.TryAddSingleton<IDomainEventDispatcher, NullDomainEventDispatcher>();

        services.AddScoped<SystemXDbContextDependencies<TContext>>(sp => new(
            options,
            sp.GetRequiredService<IClock>(),
            sp.GetRequiredService<IDomainEventDispatcher>(),
            options.IgnoreTenancy ? null : sp.GetService<ITenantContext>(),
            sp.GetService<ICurrentUser>(),
            sp.GetService<ITenantSchemaProvider>()));

        services.AddScoped<SystemXDbContextDependencies>(
            sp => sp.GetRequiredService<SystemXDbContextDependencies<TContext>>());

        services.AddDbContext<TContext>((sp, builder) => ConfigureBuilder(sp, builder, options));

        if (options.RegisterRepositories)
        {
            services.AddScoped<SystemXDbContext>(sp => sp.GetRequiredService<TContext>());
            services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<TContext>());
            services.AddScoped(typeof(IReadRepository<,>), typeof(EfReadRepository<,>));
            services.AddScoped(typeof(IRepository<,>), typeof(EfRepository<,>));
            services.AddScoped<IRepositoryFactory, EfRepositoryFactory>();
            services.AddScoped<IReadConnectionFactory, EfReadConnectionFactory>();

            services.AddScoped<IDatabaseConnectionFactory>(sp => new DatabaseConnectionFactory(
                options,
                sp.GetService<ITenantConnectionResolver>(),
                options.IgnoreTenancy ? null : sp.GetService<ITenantContext>()));

            services.AddScoped<IDatabaseFactory>(sp => new DatabaseFactory<TContext>(
                sp.GetRequiredService<IServiceScopeFactory>(),
                options,
                options.IgnoreTenancy ? null : sp.GetService<ITenantContext>()));
        }

        return services;
    }

    private static void ConfigureBuilder(IServiceProvider sp, DbContextOptionsBuilder builder, SystemXEfCoreOptions options)
    {
        var tenant = options.IgnoreTenancy ? null : sp.GetService<ITenantContext>()?.Tenant;
        var connectionString = ConnectionStringResolver.Resolve(
            options,
            sp.GetService<ITenantConnectionResolver>(),
            tenant,
            ConnectionStringResolver.DefaultName);

        options.ConfigureProvider!(builder, connectionString);
        builder.ReplaceService<IModelCacheKeyFactory, SystemXModelCacheKeyFactory>();
        options.ConfigureDbContext?.Invoke(sp, builder);
    }
}
