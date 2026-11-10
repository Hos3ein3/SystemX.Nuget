using Microsoft.Extensions.Configuration;
using SystemX.Application.Abstractions.MultiTenancy;
using SystemX.SharedKernel.MultiTenancy;

namespace SystemX.Infrastructure.MultiTenancy.Stores;

/// <summary>
/// Tenant store reading tenant configurations from .NET <see cref="IConfiguration"/> (e.g. appsettings.json section "SystemX:Tenants").
/// </summary>
public sealed class ConfigurationTenantStore : ITenantStore
{
    private readonly InMemoryTenantStore _innerStore = new();

    /// <summary>
    /// Initializes a new instance of <see cref="ConfigurationTenantStore"/> by reading the configuration section.
    /// </summary>
    /// <param name="configuration">The root configuration.</param>
    /// <param name="sectionName">The section name (defaults to "SystemX:Tenants").</param>
    public ConfigurationTenantStore(IConfiguration configuration, string sectionName = "SystemX:Tenants")
    {
        ArgumentNullException.ThrowIfNull(configuration);

        IConfigurationSection section = configuration.GetSection(sectionName);
        foreach (IConfigurationSection child in section.GetChildren())
        {
            string? idStr = child["Id"] ?? child.Key;
            string? name = child["Name"] ?? idStr;
            string? modelStr = child["Model"];
            string? identifier = child["Identifier"];
            string? schema = child["Schema"];
            string? connectionString = child["ConnectionString"];
            string? databaseName = child["DatabaseName"];
            bool isActive = !bool.TryParse(child["IsActive"], out bool active) || active;

            TenancyModel model = Enum.TryParse(modelStr, ignoreCase: true, out TenancyModel parsedModel)
                ? parsedModel
                : TenancyModel.Row;

            Dictionary<string, string> properties = [];
            IConfigurationSection propsSection = child.GetSection("Properties");
            foreach (IConfigurationSection prop in propsSection.GetChildren())
            {
                if (prop.Value != null)
                {
                    properties[prop.Key] = prop.Value;
                }
            }

            if (!string.IsNullOrWhiteSpace(idStr))
            {
                TenantInfo tenant = new(
                    new TenantId(idStr),
                    name ?? idStr,
                    model,
                    identifier,
                    schema,
                    connectionString,
                    databaseName,
                    isActive,
                    properties.Count > 0 ? properties : null);

                _innerStore.AddOrUpdate(tenant);
            }
        }
    }

    /// <inheritdoc />
    public ValueTask<TenantInfo?> FindByIdAsync(TenantId id, CancellationToken ct = default) =>
        _innerStore.FindByIdAsync(id, ct);

    /// <inheritdoc />
    public ValueTask<TenantInfo?> FindByIdentifierAsync(string identifier, CancellationToken ct = default) =>
        _innerStore.FindByIdentifierAsync(identifier, ct);

    /// <inheritdoc />
    public ValueTask<IReadOnlyList<TenantInfo>> ListAsync(CancellationToken ct = default) =>
        _innerStore.ListAsync(ct);
}
