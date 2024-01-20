namespace Shared;

public static class EnumerableExtensions
{
    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerable) =>
        enumerable.SelectMany(x => x);

    public static IEnumerable<T> TakeWhileInclusive<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        foreach (var item in source)
        {
            if (predicate(item))
            {
                yield return item;
            }
            else
            {
                yield return item;
                yield break;
            }
        }
    }

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> enumerable) =>
        enumerable
            .Where(elem => elem is not null)
            .Select(elem => elem!);

    public static IEnumerable<T> IntersectMultiple<T>(this IEnumerable<IEnumerable<T>> enumerable)
    {
        if (!enumerable.Any())
        {
            return new List<T>();
        }

        return enumerable
            .Skip(1)
            .Aggregate(
                new HashSet<T>(enumerable.First()),
                (h, e) =>
                {
                    h.IntersectWith(e);
                    return h;
                }
            );
    }
}