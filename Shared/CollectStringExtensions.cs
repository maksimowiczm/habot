namespace Shared;

public static class CollectStringExtensions
{
    public static string CollectString(this IEnumerable<char> sequence) => new(sequence.ToArray());
    public static string CollectString(this IEnumerable<string> sequence) => sequence.Aggregate((a, b) => a + b);
}