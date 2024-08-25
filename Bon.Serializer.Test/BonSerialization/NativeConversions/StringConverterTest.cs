using System.Globalization;

namespace Bon.Serializer.Test.BonSerialization.NativeConversions;

public sealed class StringConverterTest : BonSerializerTestBase
{
    [Theory]
    [InlineData("", null)]
    [InlineData("-9223372036854775810", null)]
    [InlineData("-9223372036854775809", null)]
    [InlineData("-9223372036854775808", long.MinValue)]
    [InlineData("1", 1L)]
    [InlineData("+1", 1L)]
    [InlineData(" \r\n\t1\r\n\t ", 1L)]
    [InlineData("1L", null)]
    [InlineData("1.0", null)]
    [InlineData("1,0", null)]
    [InlineData("0x1", null)]
    [InlineData("1,000", null)]
    [InlineData("1_000", null)]
    [InlineData("1000", 1000L)]
    [InlineData("9223372036854775807", long.MaxValue)]
    [InlineData("9223372036854775808", null)]
    [InlineData("9223372036854775809", null)]
    public void StringToNullableLong(string input, long? expected) => DeserializeSlow(input, expected);

    [Theory]
    [InlineData(double.NaN, "NaN")]
    [InlineData(double.NegativeInfinity, "-Infinity")]
    [InlineData(double.MinValue, "-1.7976931348623157E+308")]
    [InlineData(0.0, "0")]
    [InlineData(1.5, "1.5")]
    [InlineData(double.MaxValue, "1.7976931348623157E+308")]
    [InlineData(double.PositiveInfinity, "Infinity")]
    public void DoubleToString(double input, string expected) => DeserializeSlow(input, expected);

    [Theory]
    [InlineData("", null)]
    [InlineData("NaN", double.NaN)]
    [InlineData("-Infinity", double.NegativeInfinity)]
    [InlineData("-1.7976931348623157E+308", double.MinValue)]
    [InlineData("0", 0.0)]
    [InlineData("1.5", 1.5)]
    [InlineData("+1.5", 1.5)]
    [InlineData(" \r\n\t1.5\r\n\t ", 1.5)]
    [InlineData("1,000", 1000.0)]
    [InlineData("1.7976931348623157E+308", double.MaxValue)]
    [InlineData("Infinity", double.PositiveInfinity)]
    public void StringToNullableDouble(string input, double? expected) => DeserializeSlow(input, expected);

    [Theory]
    [InlineData(long.MinValue)]
    [InlineData(long.MaxValue)]
    public void LongRoundTrip(long value) => RoundTrip(value, x => x.ToString(Culture));

    [Fact] public void ULongRoundTrip() => RoundTrip(ulong.MaxValue, x => x.ToString(Culture));

    [Theory]
    [InlineData(float.NaN)]
    [InlineData(float.NegativeInfinity)]
    [InlineData(float.MinValue)]
    [InlineData(float.MaxValue)]
    [InlineData(float.PositiveInfinity)]
    public void FloatRoundTrip(float value) => RoundTrip(value, x => ((double)x).ToString(Culture));

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.NegativeInfinity)]
    [InlineData(double.MinValue)]
    [InlineData(double.MaxValue)]
    [InlineData(double.PositiveInfinity)]
    public void DoubleRoundTrip(double value) => RoundTrip(value, x => x.ToString(Culture));

    [Theory]
    [MemberData(nameof(DecimalRoundTripData))]
    public void DecimalRoundTrip(decimal value) => RoundTrip(value, x => x.ToString(Culture));

    public static TheoryData<decimal> DecimalRoundTripData => new() {
        { decimal.MinValue },
        { decimal.MaxValue },
    };

    [Theory]
    [MemberData(nameof(NullableDecimalRoundTripData))]
    public void NullableDecimalRoundTrip(decimal? value) => RoundTrip(value, x => x?.ToString(Culture) ?? "");

    public static TheoryData<decimal?> NullableDecimalRoundTripData => new() {
        { decimal.MinValue },
        { null },
    };

    [Fact] public void GuidRoundTrip() => RoundTrip(TestHelper.Guid);

    private void RoundTrip<T>(T value, Func<T, string?>? toString = null)
    {
        toString ??= (T x) => x?.ToString();
        DeserializeSlow(value, toString(value));
        DeserializeSlow(toString(value), value);
    }

    private static CultureInfo Culture => CultureInfo.InvariantCulture;
}
