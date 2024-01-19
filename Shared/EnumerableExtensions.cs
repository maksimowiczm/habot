namespace Shared;

public static class EnumerableExtensions
{
    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerable) =>
        enumerable.SelectMany(x => x);
}