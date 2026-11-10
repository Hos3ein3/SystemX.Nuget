using SystemX.SharedKernel.Domain;
using SystemX.SharedKernel.Results;

namespace SystemX.SharedKernel.Concurrency;

/// <summary>
/// Exception thrown when an optimistic concurrency conflict is detected.
/// </summary>
public sealed class ConcurrencyException : DomainException
{
    /// <summary>
    /// Gets the entity type name involved in the conflict.
    /// </summary>
    public string? EntityType { get; }

    /// <summary>
    /// Gets the string representation of the entity identifier involved in the conflict.
    /// </summary>
    public string? EntityId { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="ConcurrencyException"/>.
    /// </summary>
    /// <param name="entityType">The type name of the entity.</param>
    /// <param name="entityId">The identifier of the entity.</param>
    /// <param name="innerException">The underlying exception.</param>
    public ConcurrencyException(string? entityType, string? entityId, Exception? innerException = null)
        : base($"A concurrency conflict occurred for entity '{entityType}' with ID '{entityId}'.", innerException)
    {
        EntityType = entityType;
        EntityId = entityId;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ConcurrencyException"/> with a custom message.
    /// </summary>
    public ConcurrencyException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ConcurrencyException"/> with a custom message and inner exception.
    /// </summary>
    public ConcurrencyException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a default instance of <see cref="ConcurrencyException"/>.
    /// </summary>
    public ConcurrencyException() : base("A concurrency conflict occurred.")
    {
    }

    /// <summary>
    /// Creates a conflict <see cref="Error"/> representing a concurrency collision.
    /// </summary>
    /// <param name="entityName">The name of the entity.</param>
    /// <returns>A conflict error instance.</returns>
    public static Error ConcurrencyConflict(string entityName) =>
        Error.Conflict("Concurrency.Conflict", $"A concurrency conflict occurred while updating {entityName}. The record was modified by another operation.");
}
