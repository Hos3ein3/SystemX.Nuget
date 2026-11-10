using Microsoft.Extensions.DependencyInjection;
using SystemX.Application.Abstractions.MultiTenancy;

namespace SystemX.Infrastructure.Localization.Strategies;

/// <summary>
/// Resolves culture from the active tenant's metadata properties when an <see cref="ITenantContext"/> is registered.
/// </summary>
public sealed class TenantCultureStrategy : ICultureResolutionStrategy
{
    private readonly IServiceProvider _serviceProvider;
    private readonly LocalizationOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="TenantCultureStrategy"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to resolve the optional tenant context.</param>
    /// <param name="options">The localization options.</param>
    public TenantCultureStrategy(IServiceProvider serviceProvider, LocalizationOptions options)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public int Order => 500;

    /// <inheritdoc />
    public ValueTask<string?> ResolveAsync(CultureResolutionContext context, CancellationToken cancellationToken = default)
    {
        var tenantContext = _serviceProvider.GetService<ITenantContext>();
        if (tenantContext?.Tenant?.Properties != null &&
            tenantContext.Tenant.Properties.TryGetValue(_options.TenantPropertyKey, out var culture) &&
            !string.IsNullOrWhiteSpace(culture))
        {
            return ValueTask.FromResult<string?>(culture);
        }

        return ValueTask.FromResult<string?>(null);
    }
}
