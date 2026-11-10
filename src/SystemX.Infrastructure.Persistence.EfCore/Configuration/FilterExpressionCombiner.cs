using System.Linq.Expressions;

namespace SystemX.Infrastructure.Persistence.EfCore.Configuration;

internal static class FilterExpressionCombiner
{
    public static Expression<Func<T, bool>> And<T>(
        Expression<Func<T, bool>>? left,
        Expression<Func<T, bool>> right)
    {
        ArgumentNullException.ThrowIfNull(right);

        if (left is null)
        {
            return right;
        }

        var parameter = left.Parameters[0];
        var visitor = new ReplaceParameterVisitor(right.Parameters[0], parameter);
        var rightBody = visitor.Visit(right.Body)!;

        var body = Expression.AndAlso(left.Body, rightBody);
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    private sealed class ReplaceParameterVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _from;
        private readonly ParameterExpression _to;

        public ReplaceParameterVisitor(ParameterExpression from, ParameterExpression to)
        {
            _from = from;
            _to = to;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _from ? _to : base.VisitParameter(node);
        }
    }
}
