using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;

namespace Paging;

public enum FilterOperator
{
    Equals,
    NotEqual,
    GreaterThan,
    LessThan,
    GreaterThanOrEqual,
    LessThanOrEqual,
    Contains
}

public record Filter(string PropertyName, FilterOperator Operation, object Value);

public static class FilterExtensions
{
    public static Expression<Func<T, bool>> AsOrExpression<T>(this Filter[] filters)
    {
        ArgumentNullException.ThrowIfNull(filters);

        if (filters.Length < 1)
        {
            throw new InvalidOperationException($"{nameof(filters)} length must be greater than 0");
        }

        var parameter = Expression.Parameter(typeof(T));

        Expression? body = null;

        foreach (var filter in filters)
        {
            var expression = filter.GetExpression(parameter);

            body = body is null ? expression : Expression.Or(body, expression);
        }

        Debug.Assert(body is not null);

        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    private static Expression GetExpression(this Filter filter, ParameterExpression parameter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        ArgumentNullException.ThrowIfNull(parameter);

        // We Want Generate X=>X.FirstName == "Jon"

        // Here We create X.Property2 or X.Property2.Property1
        var property = filter.PropertyName.Split('.')
            .Aggregate((Expression)parameter, Expression.PropertyOrField);

        var targetType = property.Type;
        var isNullable = Nullable.GetUnderlyingType(targetType) is not null;

        if (isNullable)
        {
            targetType = Nullable.GetUnderlyingType(targetType);
            Debug.Assert(targetType is not null);
        }

        var constExpression = filter.Operation == FilterOperator.Contains
            ? Expression.Constant(filter.Value?.ToString()?.ToUpper(System.Globalization.CultureInfo.CurrentCulture), typeof(string))
            : Expression.Constant(Convert.ChangeType(filter.Value, targetType, CultureInfo.CurrentCulture), property.Type);

        Expression filterExpression;
        // Here We Create the Binary Operator Like == or > and etc.
        switch (filter.Operation)
        {
            case FilterOperator.Equals:
                filterExpression = Expression.Equal(property, constExpression);
                break;
            case FilterOperator.GreaterThanOrEqual:
                filterExpression = Expression.GreaterThanOrEqual(property, constExpression);
                break;
            case FilterOperator.LessThanOrEqual:
                filterExpression = Expression.LessThanOrEqual(property, constExpression);
                break;
            case FilterOperator.GreaterThan:
                filterExpression = Expression.GreaterThan(property, constExpression);
                break;
            case FilterOperator.LessThan:
                filterExpression = Expression.LessThan(property, constExpression);
                break;
            case FilterOperator.NotEqual:
                filterExpression = Expression.NotEqual(property, constExpression);
                break;
            case FilterOperator.Contains:
                // Here We create X.Property2.ToString()
                Expression propertyToString;

                if (isNullable)
                {
                    var hasValueProperty = Expression.Property(property, nameof(Nullable<int>.HasValue));
                    var valueProperty = Expression.Property(property, nameof(Nullable<int>.Value));
                    var valuePropertyToString = valueProperty.Type.GetMethod(nameof(ToString), Type.EmptyTypes);
                    Debug.Assert(valuePropertyToString is not null);

                    // !nullable.HasValue ? string.Empty : nullable.Value.ToString()
                    propertyToString = Expression.Condition(
                        Expression.Equal(hasValueProperty, Expression.Constant(false)),
                        Expression.Constant(string.Empty),
                        Expression.Call(valueProperty, valuePropertyToString));
                }
                else
                {
                    var propertyToStringMethod = targetType.GetMethod(nameof(ToString), Type.EmptyTypes);
                    Debug.Assert(propertyToStringMethod is not null);

                    // X.Property2.ToString()
                    propertyToString = Expression.Call(property, propertyToStringMethod);
                }

                Debug.Assert(propertyToString is not null);

                var toUpperMethodInfo = typeof(string).GetMethod(nameof(string.ToUpper), Type.EmptyTypes);
                Debug.Assert(toUpperMethodInfo is not null);

                // X.Property2.ToString().ToUpper()
                propertyToString = Expression.Call(propertyToString, toUpperMethodInfo);

                // Here we create X.Property2.ToString().Contains("Jon")
                var containsMethodInfo = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)]);
                Debug.Assert(containsMethodInfo is not null);

                // X.Property2.ToString().ToUpper().Contains("Jon")
                var containsExpression = Expression.Call(
                    propertyToString,
                    containsMethodInfo,
                    constExpression);

                // X.Property2 == null ? false : X.Property2.ToString().ToUpper().Contains("Jon")
                filterExpression = targetType.IsClass
                    ? Expression.Condition(
                        Expression.Equal(property, Expression.Constant(null)),
                        Expression.Constant(false),
                        containsExpression)
                    : containsExpression;
                break;
            default:
                throw new InvalidOperationException($"Inexhaustive {nameof(FilterOperator)} match");
        }

        return filterExpression;
    }
}
