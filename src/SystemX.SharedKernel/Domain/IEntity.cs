namespace SystemX.SharedKernel.Domain;

/// <summary>Marks an object that has an identity.</summary>
/// <typeparam name="TId">The identifier type.</typeparam>
public interface IEntity<out TId> where TId : notnull
{
    /// <summary>Gets the unique identifier.</summary>
    TId Id { get; }
}
