using System.Linq.Expressions;

namespace MeUi.Application.Utilities;

public static class PredicateBuilder
{
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        ParameterExpression parameter = Expression.Parameter(typeof(T));
        BinaryExpression body = Expression.AndAlso(
            Expression.Invoke(expr1, parameter),
            Expression.Invoke(expr2, parameter));
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        ParameterExpression parameter = Expression.Parameter(typeof(T));
        BinaryExpression body = Expression.OrElse(
            Expression.Invoke(expr1, parameter),
            Expression.Invoke(expr2, parameter));
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }
}
