namespace SystemX.SharedKernel.Auditing;

/// <summary>An object that is marked as deleted instead of being removed.</summary>
public interface ISoftDeletable
{
    /// <summary>Gets whether the object is deleted.</summary>
    bool IsDeleted { get; }

    /// <summary>Gets the UTC deletion time.</summary>
    DateTimeOffset? DeletedAt { get; }

    /// <summary>Gets who deleted the object.</summary>
    string? DeletedBy { get; }

    /// <summary>Marks the object as deleted.</summary>
    void MarkDeleted(DateTimeOffset at, string? by);

    /// <summary>Restores the object.</summary>
    void Restore();
}
