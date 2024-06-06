namespace Api.Features.Core;

public class LinqExtensions
{
    public static IEnumerable<T> Paginate<T>(IEnumerable<T> query, int page, int pageSize)
    {
        return query.Skip((page - 1) * pageSize).Take(pageSize);
    }

    public static IAsyncEnumerable<T> Paginate<T>(IAsyncEnumerable<T> query, int page, int pageSize)
    {
        return query.Skip((page - 1) * pageSize).Take(pageSize);
    }
}