namespace Bon.Serializer.Test.BonSerialization.NativeConversions;

public sealed class IntegerFloatingPointConversionTest : BonSerializerTestBase
{
    [Theory]
    [InlineData(1.99, 1)]
    [InlineData(int.MaxValue, int.MaxValue)]
    [InlineData(int.MaxValue + 1.0, int.MinValue)]
    [InlineData(int.MaxValue + 2.0, int.MinValue + 1)]
    public void DoubleToInt(double input, int expected) => DeserializeSlow(input, expected);

    [Theory]
    [MemberData(nameof(DecimalToULongData))]
    public void DecimalToULong(decimal value, ulong expected) => DeserializeSlow(value, expected);

    public static TheoryData<decimal, ulong> DecimalToULongData => new() {
        { ulong.MaxValue, ulong.MaxValue },
        { ulong.MaxValue + 1m, ulong.MaxValue },
    };

    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    public void IntToDouble(int value) => DeserializeSlow(value, (double)value);

    [Fact] public void ULongToDecimal() => DeserializeSlow(ulong.MaxValue, (decimal)ulong.MaxValue);
    [Fact] public void ULongToDouble() => DeserializeSlow(ulong.MaxValue, (double)ulong.MaxValue);
}
