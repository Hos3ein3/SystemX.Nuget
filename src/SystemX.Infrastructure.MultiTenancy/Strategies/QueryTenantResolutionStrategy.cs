using Microsoft.Extensions.Options;
using SystemX.Application.Abstractions.MultiTenancy;

namespace SystemX.Infrastructure.MultiTenancy.Strategies;

/// <summary>
/// Resolves tenant identifier from query string parameters.
/// </summary>
public sealed class QueryTenantResolutionStrategy(IOptions<MultiTenancyOptions> options) : ITenantResolutionStrategy
{
    private readonly MultiTenancyOptions _options = options?.Value ?? new MultiTenancyOptions();

    /// <inheritdoc />
    public int Order => 500;

    /// <inheritdoc />
    public ValueTask<string?> ResolveIdentifierAsync(TenantResolutionContext context, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Query.TryGetValue(_options.QueryKey, out string? value) && !string.IsNullOrWhiteSpace(value))
        {
            return ValueTask.FromResult<string?>(value.Trim());
        }

        return ValueTask.FromResult<string?>(null);
    }
}
