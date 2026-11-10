using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using SystemX.Application.Abstractions.Localization;

namespace SystemX.Infrastructure.Localization;

/// <summary>
/// ASP.NET Core middleware that resolves the request culture using registered strategies and scopes the ambient culture for the request pipeline.
/// </summary>
public sealed class LocalizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly LocalizationOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizationMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="options">The localization options.</param>
    public LocalizationMiddleware(RequestDelegate next, LocalizationOptions options)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Executes the middleware.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <param name="currentCulture">The ambient current culture service.</param>
    /// <param name="strategies">The ordered culture resolution strategies.</param>
    /// <returns>A task representing the middleware execution.</returns>
    public async Task InvokeAsync(
        HttpContext context,
        ICurrentCulture currentCulture,
        IEnumerable<ICultureResolutionStrategy> strategies)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(currentCulture);
        ArgumentNullException.ThrowIfNull(strategies);

        var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var header in context.Request.Headers)
        {
            headers[header.Key] = header.Value.ToString();
        }

        var query = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var q in context.Request.Query)
        {
            query[q.Key] = q.Value.ToString();
        }

        var cookies = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var cookie in context.Request.Cookies)
        {
            cookies[cookie.Key] = cookie.Value;
        }

        var claims = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (context.User?.Claims != null)
        {
            foreach (var claim in context.User.Claims)
            {
                claims.TryAdd(claim.Type, claim.Value);
            }
        }

        var resolutionContext = new CultureResolutionContext(headers, query, cookies, claims);

        string? resolvedCulture = null;
        foreach (var strategy in strategies.OrderBy(s => s.Order))
        {
            var raw = await strategy.ResolveAsync(resolutionContext, context.RequestAborted).ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(raw))
            {
                var matched = CultureMatcher.Match(raw, _options.SupportedCultures, string.Empty);
                if (!string.IsNullOrEmpty(matched))
                {
                    resolvedCulture = matched;
                    break;
                }
            }
        }

        resolvedCulture ??= _options.DefaultCulture;

        if (_options.AddContentLanguageHeader)
        {
            context.Response.Headers["Content-Language"] = resolvedCulture;
        }

        using (currentCulture.BeginScope(resolvedCulture))
        {
            await _next(context).ConfigureAwait(false);
        }
    }
}

/// <summary>
/// Extension methods for adding localization middleware to the ASP.NET Core pipeline.
/// </summary>
public static class LocalizationApplicationBuilderExtensions
{
    /// <summary>
    /// Adds SystemX culture resolution and scoping middleware to the application request pipeline.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder for chaining.</returns>
    public static IApplicationBuilder UseSystemXLocalization(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        return app.UseMiddleware<LocalizationMiddleware>();
    }
}
