using System.Linq.Expressions;

namespace SystemX.Infrastructure.Persistence.EfCore.Repositories;

internal static class IncludePathBuilder
{
    public static string GetPath(LambdaExpression expression)
    {
        ArgumentNullException.ThrowIfNull(expression);

        var body = expression.Body;
        while (body is UnaryExpression { NodeType: ExpressionType.Convert or ExpressionType.ConvertChecked } unary)
        {
            body = unary.Operand;
        }

        var parts = new Stack<string>();
        while (body is MemberExpression member)
        {
            parts.Push(member.Member.Name);
            body = member.Expression;
            while (body is UnaryExpression { NodeType: ExpressionType.Convert or ExpressionType.ConvertChecked } u)
            {
                body = u.Operand;
            }
        }

        if (body is not ParameterExpression || parts.Count == 0)
        {
            throw new ArgumentException(
                $"Invalid include expression '{expression}'. Include expressions must be member access paths (e.g., x => x.Customer.Address). For collection navigation paths, use IncludeStrings.",
                nameof(expression));
        }

        return string.Join(".", parts);
    }
}
