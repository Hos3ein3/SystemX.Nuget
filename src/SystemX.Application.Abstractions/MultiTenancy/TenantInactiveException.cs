using SystemX.SharedKernel.Domain;
using SystemX.SharedKernel.MultiTenancy;

namespace SystemX.Application.Abstractions.MultiTenancy;

/// <summary>
/// Exception thrown when a tenant is resolved but its account is inactive or disabled.
/// </summary>
public sealed class TenantInactiveException : DomainException
{
    /// <summary>
    /// Gets the inactive tenant identifier.
    /// </summary>
    public TenantId? TenantId { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="TenantInactiveException"/> for the given tenant ID.
    /// </summary>
    /// <param name="tenantId">The inactive tenant ID.</param>
    public TenantInactiveException(TenantId tenantId)
        : base($"Tenant '{tenantId?.Value}' is inactive.")
    {
        TenantId = tenantId;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="TenantInactiveException"/> with a custom message.
    /// </summary>
    public TenantInactiveException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="TenantInactiveException"/> with a custom message and inner exception.
    /// </summary>
    public TenantInactiveException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a default instance of <see cref="TenantInactiveException"/>.
    /// </summary>
    public TenantInactiveException() : base("The requested tenant is inactive.")
    {
    }
}
