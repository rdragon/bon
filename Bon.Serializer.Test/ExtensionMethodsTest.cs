namespace Bon.Serializer.Test;

public sealed class ExtensionMethodsTest
{
    [Theory]
    [InlineData(typeof(int), null)]
    [InlineData(typeof(Tuple<int>), null)]
    [InlineData(typeof(KeyValuePair<int, int>), null)]
    [InlineData(typeof(Dictionary<int, int>), null)]
    [InlineData(typeof(int[]), typeof(int))]
    [InlineData(typeof(IReadOnlyList<int>), typeof(int))]
    [InlineData(typeof(IEnumerable<int>), typeof(int))]
    [InlineData(typeof(List<int>), typeof(int))]
    [InlineData(typeof(IList<int>), typeof(int))]
    [InlineData(typeof(ICollection<int>), typeof(int))]
    [InlineData(typeof(HashSet<int>), typeof(int))]
    public void TryGetElementTypeOfArray(Type type, Type? expectedType)
    {
        Assert.Equal(expectedType, type.TryGetElementTypeOfArray());
    }

    [Theory]
    [InlineData(typeof(int), null)]
    [InlineData(typeof(KeyValuePair<int, int>), null)]
    [InlineData(typeof(int[]), null)]
    [InlineData(typeof(IReadOnlyList<int>), null)]
    [InlineData(typeof(Dictionary<int, int>), typeof(int))]
    [InlineData(typeof(IDictionary<int, int>), typeof(int))]
    [InlineData(typeof(IReadOnlyDictionary<int, int>), typeof(int))]
    public void TryGetTypeArgumentsOfDictionary(Type type, Type? expectedKeyType)
    {
        Assert.Equal(expectedKeyType, type.TryGetTypeArgumentsOfDictionary()?.KeyType);
    }
}