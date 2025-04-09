using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Paging;

public enum SortDirection
{
    Ascending,
    Descending
}

public static class IQueryableExtensions
{
    public static (IQueryable<T> items, int totalItemCount) Page<T>(
        this IEnumerable<T> query,
        int? currentPage,
        int? pageSize,
        string? sortOn = default,
        SortDirection? sortDirection = SortDirection.Ascending,
        Func<IQueryable<T>, IQueryable<T>>? buildQuery = default) where T : class
    {
        ArgumentNullException.ThrowIfNull(query);

        var page = currentPage ?? 1;
        var size = pageSize ?? 10;

        var source = query.AsQueryable();
        source = buildQuery?.Invoke(source) ?? source;
        var count = source.Count();

        if (!string.IsNullOrWhiteSpace(sortOn))
        {
            source = sortDirection == SortDirection.Descending
                ? source.OrderByDescending(sortOn)
                : source.OrderBy(sortOn);
        }

        return (source.Skip((page - 1) * size).Take(size), count);
    }

    public static async Task<(IQueryable<T> items, int totalItemCount)> PageAsync<T>(
        this IQueryable<T> query,
        int? currentPage,
        int? pageSize,
        string? sortOn = default,
        SortDirection? sortDirection = SortDirection.Ascending,
        Func<IQueryable<T>, IQueryable<T>>? buildQuery = default) where T : class
    {
        ArgumentNullException.ThrowIfNull(query);

        var page = currentPage ?? 1;
        var size = pageSize ?? 10;

        var source = buildQuery?.Invoke(query) ?? query;
        var count = await source.CountAsync().ConfigureAwait(false);

        if (!string.IsNullOrWhiteSpace(sortOn))
        {
            source = sortDirection == SortDirection.Descending
                ? source.OrderByDescending(sortOn)
                : source.OrderBy(sortOn);
        }

        return (source.Skip((page - 1) * size).Take(size), count);
    }

    public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(propertyName);
        return source.OrderBy(ToLambda<T>(propertyName));
    }

    public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(propertyName);
        return source.OrderByDescending(ToLambda<T>(propertyName));
    }

    private static Expression<Func<T, object>> ToLambda<T>(string propertyName)
    {
        var parameter = Expression.Parameter(typeof(T));
        var property = propertyName.Split('.')
            .Aggregate((Expression)parameter, Expression.PropertyOrField);

        var propAsObject = Expression.Convert(property, typeof(object));
        return Expression.Lambda<Func<T, object>>(propAsObject, parameter);
    }
}
