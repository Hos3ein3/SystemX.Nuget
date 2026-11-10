using SystemX.SharedKernel.Domain;
using SystemX.SharedKernel.MultiTenancy;

namespace SystemX.Application.Abstractions.MultiTenancy;

/// <summary>
/// Exception thrown when a tenant identifier was requested but does not exist in the tenant store.
/// </summary>
public sealed class TenantNotFoundException : DomainException
{
    /// <summary>
    /// Gets the tenant identifier that was not found, if known.
    /// </summary>
    public string? Identifier { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="TenantNotFoundException"/> with the missing identifier.
    /// </summary>
    /// <param name="identifier">The missing tenant identifier or code.</param>
    public TenantNotFoundException(string? identifier)
        : base($"Tenant with identifier '{identifier}' was not found.")
    {
        Identifier = identifier;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="TenantNotFoundException"/> with a missing <see cref="TenantId"/>.
    /// </summary>
    /// <param name="tenantId">The missing strongly-typed tenant ID.</param>
    public TenantNotFoundException(TenantId tenantId)
        : base($"Tenant with ID '{tenantId?.Value}' was not found.")
    {
        Identifier = tenantId?.Value;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="TenantNotFoundException"/> with a custom message.
    /// </summary>
    public TenantNotFoundException() : base("The requested tenant was not found.")
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="TenantNotFoundException"/> with a custom message and inner exception.
    /// </summary>
    public TenantNotFoundException(string message, Exception? innerException) : base(message, innerException)
    {
    }
}
