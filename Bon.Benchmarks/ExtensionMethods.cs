namespace Bon.Benchmarks;

public static class ExtensionMethods
{
    public static int CombineHashCodes(this IEnumerable<int> hashCodes) => hashCodes.Aggregate((x, y) => x * 97367 + y);

    public static T[] CreateValues<T>(this int n, Func<T> factoryMethod) =>
        Enumerable.Range(0, n).Select(_ => factoryMethod()).ToArray();
}
