using System.Linq.Expressions;

namespace SystemX.SharedKernel.Specifications;

/// <summary>
/// Abstract base specification that projects entity <typeparamref name="T"/> to <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="T">The source entity type.</typeparam>
/// <typeparam name="TResult">The projected result type.</typeparam>
public abstract class Specification<T, TResult> : Specification<T>, ISpecification<T, TResult>
{
    private Expression<Func<T, TResult>>? _selector;

    /// <inheritdoc />
    public Expression<Func<T, TResult>> Selector =>
        _selector ?? throw new InvalidOperationException($"Specification '{GetType().Name}' must configure a projection selector via Select().");

    /// <summary>
    /// Configures the projection mapping expression.
    /// </summary>
    /// <param name="selector">The projection selector expression.</param>
    protected void Select(Expression<Func<T, TResult>> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);
        _selector = selector;
    }
}
