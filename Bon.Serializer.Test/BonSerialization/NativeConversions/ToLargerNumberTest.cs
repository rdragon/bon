namespace Bon.Serializer.Test.BonSerialization.NativeConversions;

public sealed class ToLargerNumberTest : BonSerializerTestBase
{
    [Fact] public void ByteToInt() => DeserializeSlow(byte.MaxValue, (int)byte.MaxValue);
    [Fact] public void SByteToInt() => DeserializeSlow(sbyte.MinValue, (int)sbyte.MinValue);
    [Fact] public void ShortToInt() => DeserializeSlow(short.MinValue, (int)short.MinValue);
    [Fact] public void UShortToInt() => DeserializeSlow(ushort.MaxValue, (int)ushort.MaxValue);
    [Fact] public void IntToLong() => DeserializeSlow(int.MinValue, (long)int.MinValue);
    [Fact] public void UIntToLong() => DeserializeSlow(uint.MaxValue, (long)uint.MaxValue);
    [Fact] public void LongToULong() => DeserializeSlow(long.MaxValue, (ulong)long.MaxValue);
    [Fact] public void FloatToDouble() => DeserializeSlow(float.MaxValue, (double)float.MaxValue);
    [Fact] public void DecimalToDouble() => DeserializeSlow(decimal.MaxValue, (double)decimal.MaxValue);
}
