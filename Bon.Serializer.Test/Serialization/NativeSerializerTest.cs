// Bookmark 413211217
namespace Bon.Serializer.Test.Serialization;

public class NativeSerializerTest
{
    [Theory]
    [InlineData(null, 1)]
    [InlineData(-2_000_000, 5)]
    [InlineData(-20_000, 3)]
    [InlineData(-100, 2)]
    [InlineData(60, 1)]
    [InlineData(100, 2)]
    [InlineData(20_000, 3)]
    [InlineData(2_000_000, 5)]
    public void NullableSerializationLength(int? value, int expectedLength) => RoundTrip(value, NativeSerializer.WriteNullableInt, NativeSerializer.ReadNullableInt, expectedLength);

    private static long[] NumbersArray => [-1, 0, 253, 254, 0x7fffffff7fff7f7f];
    public static TheoryData<long> Numbers => new(NumbersArray);
    public static TheoryData<long?> NumbersNullable => new([null, .. NumbersArray]);

    [Fact]
    public void SerializeString() => RoundTrip("abc", NativeSerializer.WriteString, NativeSerializer.ReadString);

    [Theory]
    [InlineData(null)]
    [InlineData("abc")]
    public void SerializeNullableString(string? value) => RoundTrip(value, NativeSerializer.WriteString, NativeSerializer.ReadNullableString);

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void SerializeBool(bool value) => RoundTrip(value, NativeSerializer.WriteBool, NativeSerializer.ReadBool);

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    [InlineData(null)]
    public void SerializeNullableBool(bool? value) => RoundTrip(value, NativeSerializer.WriteNullableBool, NativeSerializer.ReadNullableBool);

    [Theory]
    [MemberData(nameof(Numbers))]
    public void SerializeByte(long value) => RoundTrip((byte)value, NativeSerializer.WriteByte, NativeSerializer.ReadByte);

    [Theory]
    [MemberData(nameof(NumbersNullable))]
    public void SerializeNullableByte(long? value) => RoundTrip((byte?)value, NativeSerializer.WriteNullableByte, NativeSerializer.ReadNullableByte);

    [Theory]
    [MemberData(nameof(Numbers))]
    public void SerializeShort(long value) => RoundTrip((short)value, NativeSerializer.WriteShort, NativeSerializer.ReadShort);

    [Theory]
    [MemberData(nameof(NumbersNullable))]
    public void SerializeNullableShort(long? value) => RoundTrip((short?)value, NativeSerializer.WriteNullableShort, NativeSerializer.ReadNullableShort);

    [Theory]
    [MemberData(nameof(Numbers))]
    public void SerializeInt(long value) => RoundTrip((int)value, NativeSerializer.WriteInt, NativeSerializer.ReadInt);

    [Theory]
    [MemberData(nameof(NumbersNullable))]
    public void SerializeNullableInt(long? value) => RoundTrip((int?)value, NativeSerializer.WriteNullableInt, NativeSerializer.ReadNullableInt);

    [Theory]
    [MemberData(nameof(Numbers))]
    public void SerializeLong(long value) => RoundTrip(value, NativeSerializer.WriteLong, NativeSerializer.ReadLong);

    [Theory]
    [MemberData(nameof(NumbersNullable))]
    public void SerializeNullableLong(long? value) => RoundTrip(value, NativeSerializer.WriteNullableLong, NativeSerializer.ReadNullableLong);

    [Theory]
    [MemberData(nameof(Numbers))]
    public void SerializeFloat(long value) => RoundTrip(value, NativeSerializer.WriteFloat, NativeSerializer.ReadFloat);

    [Theory]
    [MemberData(nameof(NumbersNullable))]
    public void SerializeNullableFloat(long? value) => RoundTrip(value, NativeSerializer.WriteNullableFloat, NativeSerializer.ReadNullableFloat);

    [Theory]
    [MemberData(nameof(Numbers))]
    public void SerializeDouble(long value) => RoundTrip(value, NativeSerializer.WriteDouble, NativeSerializer.ReadDouble);

    [Theory]
    [MemberData(nameof(NumbersNullable))]
    public void SerializeNullableDouble(long? value) => RoundTrip(value, NativeSerializer.WriteNullableDouble, NativeSerializer.ReadNullableDouble);

    [Theory]
    [MemberData(nameof(Numbers))]
    public void SerializeDecimal(long value) => RoundTrip(value, NativeSerializer.WriteDecimal, NativeSerializer.ReadDecimal);

    [Theory]
    [MemberData(nameof(NumbersNullable))]
    public void SerializeNullableDecimal(long? value) => RoundTrip(value, NativeSerializer.WriteNullableDecimal, NativeSerializer.ReadNullableDecimal);

    [Fact] public void SerializeGuid() => RoundTrip(TestHelper.Guid, NativeSerializer.WriteGuid, NativeSerializer.ReadGuid);
    [Fact] public void SerializeNullableGuid() => RoundTrip(TestHelper.Guid, NativeSerializer.WriteNullableGuid, NativeSerializer.ReadNullableGuid);
    [Fact] public void SerializeNullGuid() => RoundTrip(null, NativeSerializer.WriteNullableGuid, NativeSerializer.ReadNullableGuid);

    private static void RoundTrip<T>(T value, Action<BinaryWriter, T> writer, Func<BinaryReader, T> reader, int? expectedLength = null)
    {
        var stream = new MemoryStream();
        var binaryWriter = new BinaryWriter(stream);
        writer(binaryWriter, value);

        if (expectedLength is int expected)
        {
            Assert.Equal(expected, stream.Length);
        }

        stream.Position = 0;
        var binaryReader = new BinaryReader(stream);
        var actual = reader(binaryReader);
        Assert.Equal(value, actual);
    }
}
