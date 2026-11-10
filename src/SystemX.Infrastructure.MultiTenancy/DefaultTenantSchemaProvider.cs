using Microsoft.Extensions.Options;
using SystemX.Application.Abstractions.MultiTenancy;
using SystemX.SharedKernel.MultiTenancy;

namespace SystemX.Infrastructure.MultiTenancy;

/// <summary>
/// Default implementation of <see cref="ITenantSchemaProvider"/> using configured options and tenant metadata.
/// </summary>
public sealed class DefaultTenantSchemaProvider(IOptions<MultiTenancyOptions> options) : ITenantSchemaProvider
{
    private readonly MultiTenancyOptions _options = options?.Value ?? new MultiTenancyOptions();

    /// <inheritdoc />
    public string GetSchema(TenantInfo tenant)
    {
        ArgumentNullException.ThrowIfNull(tenant);

        if (!string.IsNullOrWhiteSpace(tenant.Schema))
        {
            return tenant.Schema;
        }

        string format = string.IsNullOrWhiteSpace(_options.SchemaNameFormat) ? "tenant_{0}" : _options.SchemaNameFormat;
        return string.Format(System.Globalization.CultureInfo.InvariantCulture, format, tenant.Id.Value);
    }
}
