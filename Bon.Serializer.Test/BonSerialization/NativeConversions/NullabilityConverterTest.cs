namespace Bon.Serializer.Test.BonSerialization.NativeConversions;

public sealed class NullabilityConverterTest : BonSerializerTestBase
{
    [Fact] public void NullLongToString() => DeserializeSlow((long?)null, (string?)null);
    [Fact] public void NullLongToDouble() => DeserializeSlow((long?)null, (double?)null);
    [Fact] public void NullLongToDecimal() => DeserializeSlow((long?)null, (decimal?)null);

    [Fact] public void NullDoubleToString() => DeserializeSlow((double?)null, (string?)null);
    [Fact] public void NullDoubleToLong() => DeserializeSlow((double?)null, (long?)null);
    [Fact] public void NullDoubleToDecimal() => DeserializeSlow((double?)null, (decimal?)null);

    [Fact] public void NullDecimalToString() => DeserializeSlow((decimal?)null, (string?)null);
    [Fact] public void NullDecimalToDouble() => DeserializeSlow((decimal?)null, (double?)null);
    [Fact] public void NullDecimalToULong() => DeserializeSlow((decimal?)null, (ulong?)null);
}
