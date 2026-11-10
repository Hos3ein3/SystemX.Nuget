using System.Text.RegularExpressions;
using SystemX.SharedKernel.Results;

namespace SystemX.SharedKernel.MultiTenancy;

/// <summary>
/// Encapsulates tenant configuration, isolation model, and connection metadata.
/// </summary>
/// <param name="Id">The strongly-typed tenant identifier.</param>
/// <param name="Name">The display name of the tenant.</param>
/// <param name="Model">The tenancy isolation model.</param>
/// <param name="Identifier">Optional external identifier, such as a subdomain, URL prefix, or external client key.</param>
/// <param name="Schema">The database schema name used when <paramref name="Model"/> is <see cref="TenancyModel.Schema"/>.</param>
/// <param name="ConnectionString">The database connection string used when <paramref name="Model"/> is <see cref="TenancyModel.Database"/> or as an override.</param>
/// <param name="DatabaseName">The database name used when <paramref name="Model"/> is <see cref="TenancyModel.Database"/> (for document stores like MongoDB).</param>
/// <param name="IsActive">Whether the tenant is active and permitted to process requests.</param>
/// <param name="Properties">Custom metadata dictionary associated with the tenant.</param>
public sealed record TenantInfo(
    TenantId Id,
    string Name,
    TenancyModel Model,
    string? Identifier = null,
    string? Schema = null,
    string? ConnectionString = null,
    string? DatabaseName = null,
    bool IsActive = true,
    IReadOnlyDictionary<string, string>? Properties = null)
{
    private static readonly Regex SafeIdentifierRegex = new("^[a-zA-Z0-9_]+$", RegexOptions.Compiled);

    /// <summary>
    /// Validates the combination of model, settings, and safe identifier naming rules.
    /// </summary>
    /// <returns>A <see cref="Result"/> indicating success or validation failure.</returns>
    public Result Validate()
    {
        List<ValidationFailure> failures = [];

        if (string.IsNullOrWhiteSpace(Name))
        {
            failures.Add(new ValidationFailure(nameof(Name), "Tenant.EmptyName", "Tenant Name cannot be empty."));
        }

        if (Model == TenancyModel.Schema)
        {
            if (string.IsNullOrWhiteSpace(Schema))
            {
                failures.Add(new ValidationFailure(nameof(Schema), "Tenant.SchemaRequired", "Schema name is required when TenancyModel is Schema."));
            }
            else if (!SafeIdentifierRegex.IsMatch(Schema))
            {
                failures.Add(new ValidationFailure(nameof(Schema), "Tenant.InvalidSchema", "Schema name contains unsafe characters; only letters, digits, and underscores are allowed."));
            }
        }
        else if (Model == TenancyModel.Database)
        {
            if (string.IsNullOrWhiteSpace(ConnectionString) && string.IsNullOrWhiteSpace(DatabaseName))
            {
                failures.Add(new ValidationFailure(nameof(ConnectionString), "Tenant.DatabaseSettingsRequired", "Either ConnectionString or DatabaseName is required when TenancyModel is Database."));
            }

            if (!string.IsNullOrWhiteSpace(DatabaseName) && !SafeIdentifierRegex.IsMatch(DatabaseName))
            {
                failures.Add(new ValidationFailure(nameof(DatabaseName), "Tenant.InvalidDatabaseName", "DatabaseName contains unsafe characters; only letters, digits, and underscores are allowed."));
            }
        }

        if (failures.Count > 0)
        {
            return Result.Failure(Error.Validation(failures));
        }

        return Result.Success();
    }

    /// <inheritdoc />
    public override string ToString() =>
        $"TenantInfo {{ Id = {Id}, Name = {Name}, Model = {Model}, Identifier = {Identifier}, Schema = {Schema}, ConnectionString = [REDACTED], DatabaseName = {DatabaseName}, IsActive = {IsActive} }}";
}
