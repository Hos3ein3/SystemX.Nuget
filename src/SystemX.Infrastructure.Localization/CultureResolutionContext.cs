namespace SystemX.Infrastructure.Localization;

/// <summary>
/// Transport-neutral context carrying request metadata (headers, query, cookies, claims) for culture resolution.
/// </summary>
public sealed class CultureResolutionContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CultureResolutionContext"/> class.
    /// </summary>
    /// <param name="headers">Optional dictionary of request or message headers.</param>
    /// <param name="query">Optional dictionary of query parameters.</param>
    /// <param name="cookies">Optional dictionary of cookies.</param>
    /// <param name="claims">Optional dictionary of user claims.</param>
    public CultureResolutionContext(
        IReadOnlyDictionary<string, string>? headers = null,
        IReadOnlyDictionary<string, string>? query = null,
        IReadOnlyDictionary<string, string>? cookies = null,
        IReadOnlyDictionary<string, string>? claims = null)
    {
        Headers = headers ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        Query = query ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        Cookies = cookies ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        Claims = claims ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>Gets the request or message headers.</summary>
    public IReadOnlyDictionary<string, string> Headers { get; }

    /// <summary>Gets the query string parameters.</summary>
    public IReadOnlyDictionary<string, string> Query { get; }

    /// <summary>Gets the cookies.</summary>
    public IReadOnlyDictionary<string, string> Cookies { get; }

    /// <summary>Gets the claims.</summary>
    public IReadOnlyDictionary<string, string> Claims { get; }

    /// <summary>
    /// Creates a resolution context from transport headers (reading x-culture / Accept-Language).
    /// </summary>
    /// <param name="headers">The headers dictionary.</param>
    /// <returns>A new <see cref="CultureResolutionContext"/> populated with the given headers.</returns>
    public static CultureResolutionContext FromHeaders(IReadOnlyDictionary<string, string> headers)
    {
        ArgumentNullException.ThrowIfNull(headers);
        return new CultureResolutionContext(headers: headers);
    }
}
