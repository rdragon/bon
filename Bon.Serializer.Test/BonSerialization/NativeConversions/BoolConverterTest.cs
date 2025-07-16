namespace Bon.Serializer.Test.BonSerialization.NativeConversions;

public sealed class BoolConverterTest : BonSerializerTestBase
{
    [Theory]
    [InlineData(false, 0)]
    [InlineData(true, 1)]
    public void BoolToInt(bool value, int expected) => DeserializeSlow(value, expected);

    [Theory]
    [InlineData(-2, true)]
    [InlineData(-1, true)]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(2, true)]
    public void IntToBool(int value, bool expected) => DeserializeSlow(value, expected);

    [Theory]
    [InlineData(-3.7, true)]
    [InlineData(-1, true)]
    [InlineData(-0.9, false)]
    [InlineData(0, false)]
    [InlineData(0.9, false)]
    [InlineData(1, true)]
    [InlineData(3.7, true)]
    public void DoubleToBool(double value, bool expected) => DeserializeSlow(value, expected);

    [Theory]
    [InlineData(false, "0")]
    [InlineData(true, "1")]
    public void BoolToString(bool input, string expected) => DeserializeSlow(input, expected);

    [Theory]
    [InlineData(false, "0")]
    [InlineData(true, "1")]
    [InlineData(null, null)]
    public void NullableBoolToString(bool? input, string? expected) => DeserializeSlow(input, expected);

    [Theory]
    [InlineData("-3.7", false)]
    [InlineData("-1", false)]
    [InlineData("0", false)]
    [InlineData("1", true)]
    [InlineData(" 1 ", true)]
    [InlineData("3", true)]
    [InlineData("3.7", false)]
    [InlineData("", false)]
    [InlineData("false", false)]
    [InlineData("True", false)]
    public void StringToBool(string input, bool expected) => DeserializeSlow(input, expected);

    [Theory]
    [InlineData("-1", null)]
    [InlineData("0", false)]
    [InlineData("1", true)]
    [InlineData(" 1 ", true)]
    [InlineData("2", true)]
    [InlineData("", null)]
    [InlineData("false", null)]
    [InlineData("True", null)]
    public void StringToNullableBool(string input, bool? expected) => DeserializeSlow(input, expected);
}
