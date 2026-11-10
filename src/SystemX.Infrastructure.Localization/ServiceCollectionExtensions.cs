using Microsoft.Extensions.DependencyInjection.Extensions;
using SystemX.Application.Abstractions.Localization;
using SystemX.Infrastructure.Localization;
using SystemX.Infrastructure.Localization.Strategies;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for registering SystemX localization services.
/// </summary>
public static class LocalizationServiceCollectionExtensions
{
    /// <summary>
    /// Registers SystemX localization services, current culture provider, default strategies, and options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Optional configuration action for localization options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSystemXLocalization(
        this IServiceCollection services,
        Action<LocalizationOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = new LocalizationOptions();
        configure?.Invoke(options);

        services.TryAddSingleton(options);
        services.TryAddSingleton<ICurrentCulture, CurrentCulture>();

        services.AddCultureStrategy<QueryCultureStrategy>();
        services.AddCultureStrategy<CookieCultureStrategy>();
        services.AddCultureStrategy<ClaimCultureStrategy>();
        services.AddCultureStrategy<AcceptLanguageStrategy>();
        services.AddCultureStrategy<TenantCultureStrategy>();

        return services;
    }

    /// <summary>
    /// Registers a custom culture resolution strategy.
    /// </summary>
    /// <typeparam name="TStrategy">The strategy implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddCultureStrategy<TStrategy>(this IServiceCollection services)
        where TStrategy : class, ICultureResolutionStrategy
    {
        ArgumentNullException.ThrowIfNull(services);
        services.TryAddEnumerable(ServiceDescriptor.Transient<ICultureResolutionStrategy, TStrategy>());
        return services;
    }
}
