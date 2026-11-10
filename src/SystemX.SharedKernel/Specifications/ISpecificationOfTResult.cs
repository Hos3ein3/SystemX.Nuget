using System.Linq.Expressions;

namespace SystemX.SharedKernel.Specifications;

/// <summary>
/// Encapsulates query logic and projection for entities of type <typeparamref name="T"/> to <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
/// <typeparam name="TResult">The projected result type.</typeparam>
public interface ISpecification<T, TResult> : ISpecification<T>
{
    /// <summary>
    /// Gets the projection mapping expression from <typeparamref name="T"/> to <typeparamref name="TResult"/>.
    /// </summary>
    Expression<Func<T, TResult>> Selector { get; }
}
