using SystemX.SharedKernel.Identifiers;

namespace SystemX.SharedKernel.MultiTenancy;

/// <summary>
/// Strongly-typed identifier for a tenant.
/// </summary>
/// <param name="Value">The underlying tenant identifier string.</param>
public sealed record TenantId(string Value) : StringId(Value), IStringIdFactory<TenantId>
{
    /// <inheritdoc />
    protected override int? MaxLength => 128;

    /// <summary>
    /// Creates a new <see cref="TenantId"/> instance from the specified string value.
    /// </summary>
    /// <param name="value">The tenant string value.</param>
    /// <returns>A new <see cref="TenantId"/> instance.</returns>
    public static TenantId Create(string value) => new(value);
}
