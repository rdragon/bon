namespace Bon.Serializer.Test.BonSerialization;

// Bookmark 659516266 (char serialization)
public sealed class CharTest : BonSerializerTestBase
{
    [Fact] public void SkipChar() => DeserializeSlow(WithChar, 0);
    [Fact] public void SkipNullableChar() => DeserializeSlow(WithNullableChar, 0);
    [Fact] public void SkipDefaultNullableChar() => DeserializeSlow(DefaultWithNullableChar, 0);

    [Fact] public void DefaultNullableCharRoundTrip() => RoundTripSlow(DefaultNullableChar);
    [Fact] public void DefaultWithNullableCharRoundTrip() => RoundTripFast(DefaultWithNullableChar);

    [Fact] public void CharRoundTrip() => RoundTripSlow(Char);
    [Fact] public void NullableCharRoundTrip() => RoundTripSlow(NullableChar);
    [Fact] public void WithCharRoundTrip() => RoundTripFast(WithChar);
    [Fact] public void WithNullableCharRoundTrip() => RoundTripFast(WithNullableChar);

    [Fact] public void ByteToChar() => DeserializeSlow((byte)Char, Char);
    [Fact] public void NullableByteToNullableChar() => DeserializeSlow((byte?)Char, NullableChar);
    [Fact] public void StringToChar() => DeserializeSlow("97", 'a');
    [Fact] public void StringToNullableChar() => DeserializeSlow("a", (char?)null);
}
