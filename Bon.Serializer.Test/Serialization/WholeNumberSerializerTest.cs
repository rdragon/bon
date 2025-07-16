namespace Bon.Serializer.Test.Serialization;

public sealed class WholeNumberSerializerTest
{
    [Theory]
    [InlineData(10)]
    [InlineData(1000)]
    [InlineData(100000)]
    [InlineData(10000000)]
    [InlineData(1000000000)]
    [InlineData(100000000000)]
    [InlineData(10000000000000)]
    public void Read(ulong value)
    {
        RoundTrip(value, WholeNumberSerializer.Write, WholeNumberSerializer.Read);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(127, 1)]
    [InlineData(128, 2)]
    [InlineData(16383, 2)]
    [InlineData(16384, 3)]
    [InlineData(2097151, 3)]
    [InlineData(2097152, 5)]
    [InlineData(68719476735, 5)]
    [InlineData(68719476736, 9)]
    [InlineData(ulong.MaxValue, 9)]
    public void SerializationLength(ulong value, int expectedLength)
    {
        RoundTrip(value, WholeNumberSerializer.Write, WholeNumberSerializer.Read, expectedLength);
    }

    [Theory]
    [InlineData(null, 1)]
    [InlineData(0UL, 1)]
    [InlineData(16383UL, 2)]
    [InlineData(16384UL, 3)]
    [InlineData(68719476735UL, 5)]
    [InlineData(68719476736UL, 9)]
    public void ReadNullable(ulong? value, int expectedLength)
    {
        RoundTrip(value, WholeNumberSerializer.Write, WholeNumberSerializer.Read, expectedLength);
    }

    [Theory]
    [InlineData(long.MinValue)]
    [InlineData(long.MinValue + 1)]
    [InlineData(long.MinValue + 2)]
    [InlineData(long.MinValue / 2 - 2)]
    [InlineData(long.MinValue / 2 - 1)]
    [InlineData(long.MinValue / 2)]
    [InlineData(long.MinValue / 2 + 1)]
    [InlineData(long.MinValue / 2 + 2)]
    [InlineData(-2)]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(long.MaxValue / 2 - 2)]
    [InlineData(long.MaxValue / 2 - 1)]
    [InlineData(long.MaxValue / 2)]
    [InlineData(long.MaxValue / 2 + 1)]
    [InlineData(long.MaxValue / 2 + 2)]
    [InlineData(long.MaxValue - 2)]
    [InlineData(long.MaxValue - 1)]
    [InlineData(long.MaxValue)]
    public void ReadSigned(long value)
    {
        RoundTrip(value, WholeNumberSerializer.WriteSigned, WholeNumberSerializer.ReadSigned);
    }

    [Theory]
    [InlineData(-65, 2)]
    [InlineData(-64, 1)]
    [InlineData(63, 1)]
    [InlineData(64, 2)]
    public void SerializationLengthSigned(long value, int expectedLength)
    {
        RoundTrip(value, WholeNumberSerializer.WriteSigned, WholeNumberSerializer.ReadSigned, expectedLength);
    }

    [Theory]
    [InlineData(null, 1)]
    [InlineData(0L, 1)]
    public void ReadSignedNullable(long? value, int expectedLength)
    {
        RoundTrip(value, WholeNumberSerializer.WriteSigned, WholeNumberSerializer.ReadSigned, expectedLength);
    }

    private static void RoundTrip<T>(T value, Action<BinaryWriter, T> write, Func<BinaryReader, T> read, int? expectedLength = null)
    {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);
        write(writer, value);

        if (expectedLength is int expected)
        {
            Assert.Equal(expected, stream.Length);
        }

        stream.Position = 0;
        var reader = new BinaryReader(stream);
        Assert.Equal(value, read(reader));
    }
}
