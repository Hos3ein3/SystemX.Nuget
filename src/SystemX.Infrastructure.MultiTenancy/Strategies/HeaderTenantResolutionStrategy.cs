using Microsoft.Extensions.Options;
using SystemX.Application.Abstractions.MultiTenancy;

namespace SystemX.Infrastructure.MultiTenancy.Strategies;

/// <summary>
/// Resolves tenant identifier from incoming HTTP request headers.
/// </summary>
public sealed class HeaderTenantResolutionStrategy(IOptions<MultiTenancyOptions> options) : ITenantResolutionStrategy
{
    private readonly MultiTenancyOptions _options = options?.Value ?? new MultiTenancyOptions();

    /// <inheritdoc />
    public int Order => 100;

    /// <inheritdoc />
    public ValueTask<string?> ResolveIdentifierAsync(TenantResolutionContext context, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Headers.TryGetValue(_options.HeaderName, out string? value) && !string.IsNullOrWhiteSpace(value))
        {
            return ValueTask.FromResult<string?>(value.Trim());
        }

        return ValueTask.FromResult<string?>(null);
    }
}
