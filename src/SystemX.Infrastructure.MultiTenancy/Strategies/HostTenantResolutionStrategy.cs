using Microsoft.Extensions.Options;
using SystemX.Application.Abstractions.MultiTenancy;

namespace SystemX.Infrastructure.MultiTenancy.Strategies;

/// <summary>
/// Resolves tenant identifier from the host name (subdomain matching).
/// </summary>
public sealed class HostTenantResolutionStrategy(IOptions<MultiTenancyOptions> options) : ITenantResolutionStrategy
{
    private readonly MultiTenancyOptions _options = options?.Value ?? new MultiTenancyOptions();

    /// <inheritdoc />
    public int Order => 300;

    /// <inheritdoc />
    public ValueTask<string?> ResolveIdentifierAsync(TenantResolutionContext context, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (string.IsNullOrWhiteSpace(context.Host))
        {
            return ValueTask.FromResult<string?>(null);
        }

        string host = context.Host.Trim();

        // Remove port if present
        int colonIdx = host.IndexOf(':', StringComparison.Ordinal);
        if (colonIdx > 0)
        {
            host = host[..colonIdx];
        }

        if (!string.IsNullOrWhiteSpace(_options.HostSuffix))
        {
            string suffix = _options.HostSuffix.TrimStart('.');
            if (host.EndsWith("." + suffix, StringComparison.OrdinalIgnoreCase))
            {
                string subdomain = host[..^(suffix.Length + 1)];
                if (!string.IsNullOrWhiteSpace(subdomain))
                {
                    return ValueTask.FromResult<string?>(subdomain);
                }
            }
        }
        else
        {
            // Default subdomain: first component of dot-separated host (if more than 2 parts or not localhost/ip)
            string[] parts = host.Split('.');
            if (parts.Length > 2 && !string.IsNullOrWhiteSpace(parts[0]))
            {
                return ValueTask.FromResult<string?>(parts[0]);
            }
        }

        return ValueTask.FromResult<string?>(null);
    }
}
