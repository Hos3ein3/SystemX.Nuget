using Microsoft.Extensions.Options;
using SystemX.Application.Abstractions.MultiTenancy;

namespace SystemX.Infrastructure.MultiTenancy.Strategies;

/// <summary>
/// Resolves tenant identifier from claims present on the authenticated principal.
/// </summary>
public sealed class ClaimTenantResolutionStrategy(IOptions<MultiTenancyOptions> options) : ITenantResolutionStrategy
{
    private readonly MultiTenancyOptions _options = options?.Value ?? new MultiTenancyOptions();

    /// <inheritdoc />
    public int Order => 200;

    /// <inheritdoc />
    public ValueTask<string?> ResolveIdentifierAsync(TenantResolutionContext context, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Claims.TryGetValue(_options.ClaimType, out string? value) && !string.IsNullOrWhiteSpace(value))
        {
            return ValueTask.FromResult<string?>(value.Trim());
        }

        return ValueTask.FromResult<string?>(null);
    }
}
