using SystemX.Application.Abstractions.MultiTenancy;
using SystemX.SharedKernel.MultiTenancy;
using SystemX.SharedKernel.Results;

namespace SystemX.Infrastructure.MultiTenancy;

/// <summary>
/// Service responsible for resolving tenant identity, looking up metadata, and validating tenant status.
/// </summary>
public sealed class TenantResolver
{
    private readonly IEnumerable<ITenantResolutionStrategy> _strategies;
    private readonly ITenantStore _tenantStore;

    /// <summary>
    /// Initializes a new instance of <see cref="TenantResolver"/>.
    /// </summary>
    /// <param name="strategies">The ordered collection of resolution strategies.</param>
    /// <param name="tenantStore">The tenant store.</param>
    public TenantResolver(
        IEnumerable<ITenantResolutionStrategy> strategies,
        ITenantStore tenantStore)
    {
        ArgumentNullException.ThrowIfNull(strategies);
        ArgumentNullException.ThrowIfNull(tenantStore);

        _strategies = strategies.OrderBy(s => s.Order);
        _tenantStore = tenantStore;
    }

    /// <summary>
    /// Resolves the tenant from the resolution context, validates status and configuration, and returns the tenant info.
    /// </summary>
    /// <param name="context">The transport resolution context.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The resolved and validated tenant info, or <see langword="null"/> if no strategy resolved an identifier.</returns>
    /// <exception cref="TenantNotFoundException">Thrown when an identifier was resolved but does not exist in the store.</exception>
    /// <exception cref="TenantInactiveException">Thrown when the resolved tenant is marked inactive.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the tenant configuration fails validation.</exception>
    public async ValueTask<TenantInfo?> ResolveAsync(TenantResolutionContext context, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        string? resolvedIdentifier = null;
        foreach (ITenantResolutionStrategy strategy in _strategies)
        {
            string? id = await strategy.ResolveIdentifierAsync(context, ct).ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(id))
            {
                resolvedIdentifier = id.Trim();
                break;
            }
        }

        if (string.IsNullOrWhiteSpace(resolvedIdentifier))
        {
            return null;
        }

        TenantInfo? tenant = await _tenantStore.FindByIdentifierAsync(resolvedIdentifier, ct).ConfigureAwait(false);
        if (tenant == null)
        {
            throw new TenantNotFoundException(resolvedIdentifier);
        }

        if (!tenant.IsActive)
        {
            throw new TenantInactiveException(tenant.Id);
        }

        Result validationResult = tenant.Validate();
        if (validationResult.IsFailure)
        {
            throw new InvalidOperationException($"Tenant configuration for '{tenant.Id.Value}' is invalid: {validationResult.Error.Description}");
        }

        return tenant;
    }
}
