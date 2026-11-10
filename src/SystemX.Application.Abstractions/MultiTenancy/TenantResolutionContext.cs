namespace SystemX.Application.Abstractions.MultiTenancy;

/// <summary>
/// Transport-neutral context carrying request metadata (headers, claims, query, route, host) for tenant resolution.
/// </summary>
public sealed class TenantResolutionContext
{
    /// <summary>
    /// Gets the dictionary of incoming request or message headers.
    /// </summary>
    public IReadOnlyDictionary<string, string> Headers { get; }

    /// <summary>
    /// Gets the dictionary of claims from the authenticated user or token.
    /// </summary>
    public IReadOnlyDictionary<string, string> Claims { get; }

    /// <summary>
    /// Gets the dictionary of query string parameters.
    /// </summary>
    public IReadOnlyDictionary<string, string> Query { get; }

    /// <summary>
    /// Gets the dictionary of route values.
    /// </summary>
    public IReadOnlyDictionary<string, string> RouteValues { get; }

    /// <summary>
    /// Gets the host name from the request (e.g. "tenant1.app.com").
    /// </summary>
    public string? Host { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="TenantResolutionContext"/>.
    /// </summary>
    public TenantResolutionContext(
        IReadOnlyDictionary<string, string>? headers = null,
        IReadOnlyDictionary<string, string>? claims = null,
        IReadOnlyDictionary<string, string>? query = null,
        IReadOnlyDictionary<string, string>? routeValues = null,
        string? host = null)
    {
        Headers = headers ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        Claims = claims ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        Query = query ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        RouteValues = routeValues ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        Host = host;
    }

    /// <summary>
    /// Creates a resolution context from message or job headers.
    /// </summary>
    /// <param name="headers">The headers dictionary.</param>
    /// <returns>A new <see cref="TenantResolutionContext"/>.</returns>
    public static TenantResolutionContext FromHeaders(IReadOnlyDictionary<string, string> headers)
    {
        ArgumentNullException.ThrowIfNull(headers);
        return new TenantResolutionContext(headers: headers);
    }
}
