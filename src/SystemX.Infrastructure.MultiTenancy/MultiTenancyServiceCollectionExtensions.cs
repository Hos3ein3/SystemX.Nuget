using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SystemX.Application.Abstractions.MultiTenancy;
using SystemX.Infrastructure.MultiTenancy.Stores;
using SystemX.Infrastructure.MultiTenancy.Strategies;

namespace SystemX.Infrastructure.MultiTenancy;

/// <summary>
/// Extension methods for registering and configuring multi-tenancy services and middleware.
/// </summary>
public static class MultiTenancyServiceCollectionExtensions
{
    /// <summary>
    /// Adds multi-tenancy resolution, context accessor, and resolution strategies to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Optional configuration for multi-tenancy options.</param>
    /// <returns>A builder for configuring tenant stores and strategies.</returns>
    public static MultiTenancyBuilder AddSystemXMultiTenancy(
        this IServiceCollection services,
        Action<MultiTenancyOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (configure != null)
        {
            services.Configure(configure);
        }
        else
        {
            services.AddOptions<MultiTenancyOptions>();
        }

        // Context accessor (singleton wrapping AsyncLocal)
        TenantContextAccessor accessor = new();
        services.TryAddSingleton<ITenantContextAccessor>(accessor);
        services.TryAddSingleton<ITenantContext>(sp => sp.GetRequiredService<ITenantContextAccessor>());

        // Resolvers and Providers
        services.TryAddTransient<ITenantConnectionResolver, DefaultTenantConnectionResolver>();
        services.TryAddTransient<ITenantSchemaProvider, DefaultTenantSchemaProvider>();
        services.TryAddScoped<TenantResolver>();

        // Default strategies
        services.TryAddEnumerable(ServiceDescriptor.Transient<ITenantResolutionStrategy, HeaderTenantResolutionStrategy>());
        services.TryAddEnumerable(ServiceDescriptor.Transient<ITenantResolutionStrategy, ClaimTenantResolutionStrategy>());
        services.TryAddEnumerable(ServiceDescriptor.Transient<ITenantResolutionStrategy, HostTenantResolutionStrategy>());
        services.TryAddEnumerable(ServiceDescriptor.Transient<ITenantResolutionStrategy, RouteTenantResolutionStrategy>());
        services.TryAddEnumerable(ServiceDescriptor.Transient<ITenantResolutionStrategy, QueryTenantResolutionStrategy>());

        // Fallback store
        services.TryAddSingleton<ITenantStore, InMemoryTenantStore>();

        return new MultiTenancyBuilder(services);
    }

    /// <summary>
    /// Registers the SystemX multi-tenancy middleware into the ASP.NET Core request processing pipeline.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder for chaining.</returns>
    public static IApplicationBuilder UseSystemXMultiTenancy(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        return app.UseMiddleware<MultiTenancyMiddleware>();
    }
}

/// <summary>
/// Fluent builder for customizing multi-tenancy store and strategy registrations.
/// </summary>
public sealed class MultiTenancyBuilder(IServiceCollection services)
{
    /// <summary>
    /// Gets the underlying service collection.
    /// </summary>
    public IServiceCollection Services { get; } = services ?? throw new ArgumentNullException(nameof(services));

    /// <summary>
    /// Registers a custom tenant store implementation.
    /// </summary>
    /// <typeparam name="TStore">The store implementation type.</typeparam>
    /// <returns>The builder instance.</returns>
    public MultiTenancyBuilder AddTenantStore<TStore>() where TStore : class, ITenantStore
    {
        Services.RemoveAll<ITenantStore>();
        Services.AddSingleton<ITenantStore, TStore>();
        return this;
    }

    /// <summary>
    /// Registers a custom resolution strategy.
    /// </summary>
    /// <typeparam name="TStrategy">The resolution strategy type.</typeparam>
    /// <returns>The builder instance.</returns>
    public MultiTenancyBuilder AddResolutionStrategy<TStrategy>() where TStrategy : class, ITenantResolutionStrategy
    {
        Services.AddTransient<ITenantResolutionStrategy, TStrategy>();
        return this;
    }
}
