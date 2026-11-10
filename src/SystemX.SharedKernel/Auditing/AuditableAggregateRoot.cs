using SystemX.SharedKernel.Domain;

namespace SystemX.SharedKernel.Auditing;

/// <summary>Aggregate root with audit fields.</summary>
/// <typeparam name="TId">The identifier type.</typeparam>
public abstract class AuditableAggregateRoot<TId> : AggregateRoot<TId>, IAuditable
    where TId : notnull
{
    /// <summary>Creates an aggregate root with an id.</summary>
    protected AuditableAggregateRoot(TId id) : base(id)
    {
    }

    /// <summary>Parameterless constructor for ORMs only.</summary>
    protected AuditableAggregateRoot()
    {
    }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; private set; }

    /// <inheritdoc />
    public string? CreatedBy { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? ModifiedAt { get; private set; }

    /// <inheritdoc />
    public string? ModifiedBy { get; private set; }

    /// <inheritdoc />
    public void SetCreated(DateTimeOffset at, string? by)
    {
        CreatedAt = at;
        CreatedBy = by;
    }

    /// <inheritdoc />
    public void SetModified(DateTimeOffset at, string? by)
    {
        ModifiedAt = at;
        ModifiedBy = by;
    }
}
