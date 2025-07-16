namespace Bon.Serializer.Test;

public static class TestHelper
{
    public static bool SequenceEqual<T>(IEnumerable<T>? left, IEnumerable<T>? right)
    {
        if (left is null)
        {
            return right is null;
        }

        return right is { } && left.SequenceEqual(right);
    }

    public static Guid Guid { get; } = new(0x3a71523d, 0xa359, 0x4044, 0xae, 0xbf, 0xc3, 0x31, 0xb0, 0x5f, 0xf5, 0x72);
    public static DateTime DateTime { get; } = new(638492758577955543, DateTimeKind.Utc);
    public static DateTimeOffset DateTimeOffset { get; } = new(638492758577955543 + 108000000000, TimeSpan.FromHours(3));
    public static DateOnly DateOnly { get; } = DateOnly.FromDayNumber(738996);
    public static TimeOnly TimeOnly { get; } = new(214577955543);
    public static TimeSpan TimeSpan { get; } = new(214577955543);

    /// <summary>
    /// Returns an enumerable that does not implement <see cref="ICollection{T}"/>.
    /// </summary>
    public static IEnumerable<int> GetEnumerable()
    {
        yield return 1;
        yield return 2;
        yield return 3;
    }
}
