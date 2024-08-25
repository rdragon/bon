namespace Bon.Serializer.Test.BonSerialization.Conversions;

public sealed class EnumConversionTest : BonSerializerTestBase
{
    [Fact] public void ByteEnumToIntEnum() => DeserializeSlow(ByteEnum.A, IntEnum.A);
    [Fact] public void IntEnumToByteEnum() => DeserializeSlow(IntEnum.A, ByteEnum.A);

    [Fact] public void OutOfRangeEnumValue() => RoundTripFast((IntEnum)999);

    [Fact] public void StringToEnum1() => DeserializeSlow("169", IntEnum.A);
    [Fact] public void StringToEnum2() => DeserializeSlow("A", (IntEnum?)null);
}
