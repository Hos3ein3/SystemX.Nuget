using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SystemX.Application.Abstractions.MultiTenancy;
using SystemX.SharedKernel.MultiTenancy;

namespace SystemX.Infrastructure.MultiTenancy;

/// <summary>
/// ASP.NET Core middleware that extracts tenant context, resolves the tenant, manages ambient scope,
/// and returns RFC 7807 ProblemDetails responses on resolution failures without referencing SystemX.Infrastructure.Web.
/// </summary>
public sealed class MultiTenancyMiddleware(
    RequestDelegate next,
    IOptions<MultiTenancyOptions> options)
{
    private readonly MultiTenancyOptions _options = options?.Value ?? new MultiTenancyOptions();

    /// <summary>
    /// Invokes the middleware.
    /// </summary>
    /// <param name="httpContext">The HTTP context.</param>
    /// <param name="resolver">The tenant resolver.</param>
    /// <param name="accessor">The tenant context accessor.</param>
    /// <returns>A task representing the middleware pipeline execution.</returns>
    public async Task InvokeAsync(
        HttpContext httpContext,
        TenantResolver resolver,
        ITenantContextAccessor accessor)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(resolver);
        ArgumentNullException.ThrowIfNull(accessor);

        PathString path = httpContext.Request.Path;
        foreach (string excluded in _options.ExcludedPaths)
        {
            if (path.StartsWithSegments(excluded, StringComparison.OrdinalIgnoreCase))
            {
                await next(httpContext).ConfigureAwait(false);
                return;
            }
        }

        TenantResolutionContext context = BuildResolutionContext(httpContext);

        try
        {
            TenantInfo? tenant = await resolver.ResolveAsync(context, httpContext.RequestAborted).ConfigureAwait(false);

            if (tenant == null && _options.RequireTenant)
            {
                await WriteProblemDetailsAsync(
                    httpContext,
                    StatusCodes.Status400BadRequest,
                    "Tenant Not Resolved",
                    "A valid tenant identifier is required to access this resource.")
                    .ConfigureAwait(false);
                return;
            }

            using (accessor.BeginScope(tenant))
            {
                await next(httpContext).ConfigureAwait(false);
            }
        }
        catch (TenantNotFoundException ex)
        {
            await WriteProblemDetailsAsync(
                httpContext,
                StatusCodes.Status404NotFound,
                "Tenant Not Found",
                ex.Message)
                .ConfigureAwait(false);
        }
        catch (TenantInactiveException ex)
        {
            await WriteProblemDetailsAsync(
                httpContext,
                StatusCodes.Status403Forbidden,
                "Tenant Inactive",
                ex.Message)
                .ConfigureAwait(false);
        }
        catch (InvalidOperationException ex)
        {
            await WriteProblemDetailsAsync(
                httpContext,
                StatusCodes.Status400BadRequest,
                "Tenant Configuration Error",
                ex.Message)
                .ConfigureAwait(false);
        }
    }

    private static TenantResolutionContext BuildResolutionContext(HttpContext httpContext)
    {
        Dictionary<string, string> headers = [];
        foreach (var header in httpContext.Request.Headers)
        {
            headers[header.Key] = header.Value.ToString();
        }

        Dictionary<string, string> claims = [];
        if (httpContext.User?.Claims != null)
        {
            foreach (var claim in httpContext.User.Claims)
            {
                claims[claim.Type] = claim.Value;
            }
        }

        Dictionary<string, string> query = [];
        foreach (var q in httpContext.Request.Query)
        {
            query[q.Key] = q.Value.ToString();
        }

        Dictionary<string, string> routeValues = [];
        foreach (var r in httpContext.Request.RouteValues)
        {
            if (r.Value != null)
            {
                routeValues[r.Key] = r.Value.ToString()!;
            }
        }

        return new TenantResolutionContext(
            headers: headers,
            claims: claims,
            query: query,
            routeValues: routeValues,
            host: httpContext.Request.Host.Value);
    }

    private static async Task WriteProblemDetailsAsync(
        HttpContext context,
        int statusCode,
        string title,
        string detail)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        ProblemDetails problem = new()
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        await context.Response.WriteAsJsonAsync(problem, context.RequestAborted).ConfigureAwait(false);
    }
}
