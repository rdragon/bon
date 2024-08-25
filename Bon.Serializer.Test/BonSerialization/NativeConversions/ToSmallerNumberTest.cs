namespace Bon.Serializer.Test.BonSerialization.NativeConversions;

public sealed class ToSmallerNumberTest : BonSerializerTestBase
{
    [Theory]
    [InlineData(byte.MinValue - 2, byte.MaxValue - 1)]
    [InlineData(byte.MinValue - 1, byte.MaxValue)]
    [InlineData(byte.MinValue, byte.MinValue)]
    [InlineData(byte.MinValue + 1, 1)]
    [InlineData(byte.MaxValue, byte.MaxValue)]
    [InlineData(byte.MaxValue + 1, 0)]
    [InlineData(byte.MaxValue + 2, 1)]
    public void IntToByte(int value, byte expected) => DeserializeSlow(value, expected);

    [Theory]
    [InlineData(sbyte.MinValue - 2, sbyte.MaxValue - 1)]
    [InlineData(sbyte.MinValue - 1, sbyte.MaxValue)]
    [InlineData(sbyte.MinValue, sbyte.MinValue)]
    [InlineData(sbyte.MaxValue, sbyte.MaxValue)]
    [InlineData(sbyte.MaxValue + 1, sbyte.MinValue)]
    [InlineData(sbyte.MaxValue + 2, sbyte.MinValue + 1)]
    public void IntToSByte(int value, sbyte expected) => DeserializeSlow(value, expected);

    [Theory]
    [InlineData(short.MinValue - 2, short.MaxValue - 1)]
    [InlineData(short.MinValue - 1, short.MaxValue)]
    [InlineData(short.MinValue, short.MinValue)]
    [InlineData(short.MaxValue, short.MaxValue)]
    [InlineData(short.MaxValue + 1, short.MinValue)]
    [InlineData(short.MaxValue + 2, short.MinValue + 1)]
    public void IntToShort(int value, short expected) => DeserializeSlow(value, expected);

    [Theory]
    [InlineData(ushort.MinValue - 2, ushort.MaxValue - 1)]
    [InlineData(ushort.MinValue - 1, ushort.MaxValue)]
    [InlineData(ushort.MinValue, ushort.MinValue)]
    [InlineData(ushort.MinValue + 1, 1)]
    [InlineData(ushort.MaxValue, ushort.MaxValue)]
    [InlineData(ushort.MaxValue + 1, 0)]
    [InlineData(ushort.MaxValue + 2, 1)]
    public void IntToUShort(int value, ushort expected) => DeserializeSlow(value, expected);

    [Theory]
    [InlineData(int.MinValue - 2L, int.MaxValue - 1)]
    [InlineData(int.MinValue - 1L, int.MaxValue)]
    [InlineData(int.MinValue, int.MinValue)]
    [InlineData(int.MaxValue, int.MaxValue)]
    [InlineData(int.MaxValue + 1L, int.MinValue)]
    [InlineData(int.MaxValue + 2L, int.MinValue + 1)]
    public void LongToInt(long value, int expected) => DeserializeSlow(value, expected);

    [Theory]
    [InlineData(uint.MinValue - 2L, uint.MaxValue - 1)]
    [InlineData(uint.MinValue - 1L, uint.MaxValue)]
    [InlineData(uint.MinValue, uint.MinValue)]
    [InlineData(uint.MinValue + 1L, 1)]
    [InlineData(uint.MaxValue, uint.MaxValue)]
    [InlineData(uint.MaxValue + 1L, 0)]
    [InlineData(uint.MaxValue + 2L, 1)]
    public void LongToUInt(long value, uint expected) => DeserializeSlow(value, expected);

    [Theory]
    [InlineData(long.MaxValue, long.MaxValue)]
    [InlineData(long.MaxValue + 1UL, long.MinValue)]
    [InlineData(long.MaxValue + 2UL, long.MinValue + 1)]
    public void ULongToLong(ulong value, long expected) => DeserializeSlow(value, expected);

    [Theory]
    [InlineData(double.NaN, float.NaN)]
    [InlineData(double.NegativeInfinity, float.NegativeInfinity)]
    [InlineData(double.MinValue, float.NegativeInfinity)]
    [InlineData(-3.4028234663852886E+38, float.MinValue)]
    [InlineData(0, 0)]
    [InlineData(3.4028234663852886E+38, float.MaxValue)]
    [InlineData(double.MaxValue, float.PositiveInfinity)]
    [InlineData(double.PositiveInfinity, float.PositiveInfinity)]
    public void DoubleToFloat(double value, float expected) => DeserializeSlow(value, expected);

    [Theory]
    [MemberData((nameof(DoubleToDecimalData)))]
    public void DoubleToDecimal(double value, decimal expected) => DeserializeSlow(value, expected);

    public static TheoryData<double, decimal> DoubleToDecimalData => new() {
        { double.NaN, 0 },
        { double.NegativeInfinity, decimal.MinValue },
        { double.MinValue, decimal.MinValue },
        { -9999999999999.0, -9999999999999m },
        { 1.5, 1.5m },
        { 9999999999999.0, 9999999999999m },
        { double.MaxValue, decimal.MaxValue },
        { double.PositiveInfinity, decimal.MaxValue },
    };

    [Theory]
    [MemberData((nameof(DecimalToLongData)))]
    public void DecimalToLong(decimal value, long expected) => DeserializeSlow(value, expected);

    public static TheoryData<decimal, long> DecimalToLongData => new() {
        { decimal.MinValue, long.MinValue },
        { -9999999999999, -9999999999999 },
        { 1.5m, 1 },
        { 9999999999999, 9999999999999 },
        { decimal.MaxValue, long.MaxValue },
    };

    [Theory]
    [MemberData((nameof(DecimalToLongData)))]
    public void NullableDecimalToNullableLong(decimal value, long expected) => DeserializeSlow((decimal?)value, (long?)expected);

    [Theory]
    [MemberData((nameof(DecimalToULongData)))]
    public void DecimalToULong(decimal value, ulong expected) => DeserializeSlow(value, expected);

    public static TheoryData<decimal, ulong> DecimalToULongData => new() {
        { decimal.MinValue, ulong.MinValue },
        { -9999999999999, ulong.MinValue },
        { 1.5m, 1 },
        { 9999999999999, 9999999999999 },
        { decimal.MaxValue, ulong.MaxValue },
    };
}
