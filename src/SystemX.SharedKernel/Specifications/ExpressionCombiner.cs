using System.Linq.Expressions;

namespace SystemX.SharedKernel.Specifications;

internal static class ExpressionCombiner
{
    public static Expression<Func<T, bool>> And<T>(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {
        ParameterExpression parameter = Expression.Parameter(typeof(T), "x");

        ReplaceParameterVisitor leftVisitor = new(left.Parameters[0], parameter);
        Expression leftBody = leftVisitor.Visit(left.Body);

        ReplaceParameterVisitor rightVisitor = new(right.Parameters[0], parameter);
        Expression rightBody = rightVisitor.Visit(right.Body);

        BinaryExpression binary = Expression.AndAlso(leftBody, rightBody);
        return Expression.Lambda<Func<T, bool>>(binary, parameter);
    }

    public static Expression<Func<T, bool>> Or<T>(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {
        ParameterExpression parameter = Expression.Parameter(typeof(T), "x");

        ReplaceParameterVisitor leftVisitor = new(left.Parameters[0], parameter);
        Expression leftBody = leftVisitor.Visit(left.Body);

        ReplaceParameterVisitor rightVisitor = new(right.Parameters[0], parameter);
        Expression rightBody = rightVisitor.Visit(right.Body);

        BinaryExpression binary = Expression.OrElse(leftBody, rightBody);
        return Expression.Lambda<Func<T, bool>>(binary, parameter);
    }

    public static Expression<Func<T, bool>> Not<T>(Expression<Func<T, bool>> expression)
    {
        ParameterExpression parameter = expression.Parameters[0];
        UnaryExpression unary = Expression.Not(expression.Body);
        return Expression.Lambda<Func<T, bool>>(unary, parameter);
    }

    private sealed class ReplaceParameterVisitor(ParameterExpression oldParam, ParameterExpression newParam) : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == oldParam ? newParam : base.VisitParameter(node);
        }
    }
}
