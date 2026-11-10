using SystemX.SharedKernel.Guards;

namespace SystemX.SharedKernel.Domain;

/// <summary>
/// Base class for entities. Two entities are equal when they have the same type and the same non-default id.
/// </summary>
/// <typeparam name="TId">The identifier type.</typeparam>
public abstract class Entity<TId> : IEntity<TId>, IEquatable<Entity<TId>>
    where TId : notnull
{
    /// <summary>Creates an entity with an id.</summary>
    protected Entity(TId id)
    {
        Id = Guard.NotDefault(id);
    }

    /// <summary>Parameterless constructor for ORMs and serializers only.</summary>
    protected Entity()
    {
        Id = default!;
    }

    /// <inheritdoc />
    public TId Id { get; protected set; }

    private bool IsTransient => EqualityComparer<TId>.Default.Equals(Id, default!);

    /// <summary>Throws <see cref="BusinessRuleValidationException"/> when the rule is broken.</summary>
    protected static void CheckRule(IBusinessRule rule)
    {
        Guard.NotNull(rule);
        if (rule.IsBroken())
        {
            throw new BusinessRuleValidationException(rule);
        }
    }

    /// <inheritdoc />
    public bool Equals(Entity<TId>? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (GetType() != other.GetType())
        {
            return false;
        }

        if (IsTransient || other.IsTransient)
        {
            return false;
        }

        return EqualityComparer<TId>.Default.Equals(Id, other.Id);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Entity<TId> other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() =>
        IsTransient ? base.GetHashCode() : HashCode.Combine(GetType(), Id);

    /// <summary>Equality operator.</summary>
    public static bool operator ==(Entity<TId>? left, Entity<TId>? right) =>
        left is null ? right is null : left.Equals(right);

    /// <summary>Inequality operator.</summary>
    public static bool operator !=(Entity<TId>? left, Entity<TId>? right) => !(left == right);
}
