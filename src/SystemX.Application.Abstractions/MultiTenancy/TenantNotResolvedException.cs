using SystemX.SharedKernel.Domain;

namespace SystemX.Application.Abstractions.MultiTenancy;

/// <summary>
/// Exception thrown when an operation requires an active tenant context but none was resolved.
/// </summary>
public sealed class TenantNotResolvedException : DomainException
{
    /// <summary>
    /// Initializes a new instance of <see cref="TenantNotResolvedException"/>.
    /// </summary>
    public TenantNotResolvedException() : base("A tenant context is required for this operation, but no tenant was resolved.")
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="TenantNotResolvedException"/> with a custom message.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public TenantNotResolvedException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="TenantNotResolvedException"/> with a custom message and inner exception.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception.</param>
    public TenantNotResolvedException(string message, Exception? innerException) : base(message, innerException)
    {
    }
}
