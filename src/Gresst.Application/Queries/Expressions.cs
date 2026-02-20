using System.Linq.Expressions;

namespace Gresst.Application.Queries
{
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> AndAlso<T>(
            this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            if (first == null) return second;
            if (second == null) return first;

            var parameter = Expression.Parameter(typeof(T));
            var left = ReplaceParameter(first.Body, first.Parameters[0], parameter);
            var right = ReplaceParameter(second.Body, second.Parameters[0], parameter);
            var body = Expression.AndAlso(left, right);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        private static Expression ReplaceParameter(Expression expression, ParameterExpression toReplace, ParameterExpression replaceWith)
        {
            return new ParameterReplacer(toReplace, replaceWith).Visit(expression) ?? expression;
        }

        private class ParameterReplacer : ExpressionVisitor
        {
            private readonly ParameterExpression _from, _to;

            public ParameterReplacer(ParameterExpression from, ParameterExpression to)
            {
                _from = from;
                _to = to;
            }

            protected override Expression VisitParameter(ParameterExpression node)
                => node == _from ? _to : base.VisitParameter(node);
        }
    }
}
