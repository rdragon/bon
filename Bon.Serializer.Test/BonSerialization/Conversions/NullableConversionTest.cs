namespace Bon.Serializer.Test.BonSerialization.Conversions;

public sealed class NullableConversionTest : BonSerializerTestBase
{
    [Fact] public void WithNullableStringToWithString() => DeserializeFast(WithNullableString, WithString);
    [Fact] public void WithNullableBoolToWithBool() => DeserializeSlow(WithNullableBool, WithBool);
    [Fact] public void WithNullableByteToWithByte() => DeserializeSlow(WithNullableByte, WithByte);
    [Fact] public void WithNullableSByteToWithSByte() => DeserializeSlow(WithNullableSByte, WithSByte);
    [Fact] public void WithNullableShortToWithShort() => DeserializeSlow(WithNullableShort, WithShort);
    [Fact] public void WithNullableUShortToWithUShort() => DeserializeSlow(WithNullableUShort, WithUShort);
    [Fact] public void WithNullableIntToWithInt() => DeserializeSlow(WithNullableInt, WithInt);
    [Fact] public void WithNullableUIntToWithUInt() => DeserializeSlow(WithNullableUInt, WithUInt);
    [Fact] public void WithNullableLongToWithLong() => DeserializeSlow(WithNullableLong, WithLong);
    [Fact] public void WithNullableULongToWithULong() => DeserializeSlow(WithNullableULong, WithULong);
    [Fact] public void WithNullableFloatToWithFloat() => DeserializeSlow(WithNullableFloat, WithFloat);
    [Fact] public void WithNullableDoubleToWithDouble() => DeserializeSlow(WithNullableDouble, WithDouble);
    [Fact] public void WithNullableDecimalToWithDecimal() => DeserializeFast(WithNullableDecimal, WithDecimal);
    [Fact] public void WithNullableGuidToWithGuid() => DeserializeFast(WithNullableGuid, WithGuid);
    [Fact] public void WithNullableDayOfWeekToWithDayOfWeek() => DeserializeSlow(WithNullableDayOfWeek, WithDayOfWeek);

    [Fact] public void WithStringToWithNullableString() => DeserializeFast(WithString, WithNullableString);
    [Fact] public void WithBoolToWithNullableBool() => DeserializeSlow(WithBool, WithNullableBool);
    [Fact] public void WithByteToWithNullableByte() => DeserializeSlow(WithByte, WithNullableByte);
    [Fact] public void WithSByteToWithNullableSByte() => DeserializeSlow(WithSByte, WithNullableSByte);
    [Fact] public void WithShortToWithNullableShort() => DeserializeSlow(WithShort, WithNullableShort);
    [Fact] public void WithUShortToWithNullableUShort() => DeserializeSlow(WithUShort, WithNullableUShort);
    [Fact] public void WithIntToWithNullableInt() => DeserializeSlow(WithInt, WithNullableInt);
    [Fact] public void WithUIntToWithNullableUInt() => DeserializeSlow(WithUInt, WithNullableUInt);
    [Fact] public void WithLongToWithNullableLong() => DeserializeSlow(WithLong, WithNullableLong);
    [Fact] public void WithULongToWithNullableULong() => DeserializeSlow(WithULong, WithNullableULong);
    [Fact] public void WithFloatToWithNullableFloat() => DeserializeSlow(WithFloat, WithNullableFloat);
    [Fact] public void WithDoubleToWithNullableDouble() => DeserializeSlow(WithDouble, WithNullableDouble);
    [Fact] public void WithDecimalToWithNullableDecimal() => DeserializeFast(WithDecimal, WithNullableDecimal);
    [Fact] public void WithGuidToWithNullableGuid() => DeserializeFast(WithGuid, WithNullableGuid);
    [Fact] public void WithDayOfWeekToWithNullableDayOfWeek() => DeserializeSlow(WithDayOfWeek, WithNullableDayOfWeek);

    [Fact] public void NullableBoolToBool() => DeserializeSlow(NullableBool, Bool);
    [Fact] public void NullableByteToByte() => DeserializeSlow(NullableByte, Byte);
    [Fact] public void NullableSByteToSByte() => DeserializeSlow(NullableSByte, SByte);
    [Fact] public void NullableShortToShort() => DeserializeSlow(NullableShort, Short);
    [Fact] public void NullableUShortToUShort() => DeserializeSlow(NullableUShort, UShort);
    [Fact] public void NullableIntToInt() => DeserializeSlow(NullableInt, Int);
    [Fact] public void NullableUIntToUInt() => DeserializeSlow(NullableUInt, UInt);
    [Fact] public void NullableLongToLong() => DeserializeSlow(NullableLong, Long);
    [Fact] public void NullableULongToULong() => DeserializeSlow(NullableULong, ULong);
    [Fact] public void NullableFloatToFloat() => DeserializeSlow(NullableFloat, Float);
    [Fact] public void NullableDoubleToDouble() => DeserializeSlow(NullableDouble, Double);
    [Fact] public void NullableDecimalToDecimal() => DeserializeSlow(NullableDecimal, Decimal);
    [Fact] public void NullableGuidToGuid() => DeserializeSlow(NullableGuid, Guid);
    [Fact] public void NullableDayOfWeekToDayOfWeek() => DeserializeSlow(NullableDayOfWeek, DayOfWeek);

    [Fact] public void BoolToNullableBool() => DeserializeSlow(Bool, NullableBool);
    [Fact] public void ByteToNullableByte() => DeserializeSlow(Byte, NullableByte);
    [Fact] public void SByteToNullableSByte() => DeserializeSlow(SByte, NullableSByte);
    [Fact] public void ShortToNullableShort() => DeserializeSlow(Short, NullableShort);
    [Fact] public void UShortToNullableUShort() => DeserializeSlow(UShort, NullableUShort);
    [Fact] public void IntToNullableInt() => DeserializeSlow(Int, NullableInt);
    [Fact] public void UIntToNullableUInt() => DeserializeSlow(UInt, NullableUInt);
    [Fact] public void LongToNullableLong() => DeserializeSlow(Long, NullableLong);
    [Fact] public void ULongToNullableULong() => DeserializeSlow(ULong, NullableULong);
    [Fact] public void FloatToNullableFloat() => DeserializeSlow(Float, NullableFloat);
    [Fact] public void DoubleToNullableDouble() => DeserializeSlow(Double, NullableDouble);
    [Fact] public void DecimalToNullableDecimal() => DeserializeFast(Decimal, NullableDecimal);
    [Fact] public void GuidToNullableGuid() => DeserializeSlow(Guid, NullableGuid);
    [Fact] public void DayOfWeekToNullableDayOfWeek() => DeserializeSlow(DayOfWeek, NullableDayOfWeek);

    [Fact] public void HoldsHouseToHoldsNullableHouse() => DeserializeSlow(HoldsHouse, HoldsNullableHouse);
    [Fact] public void HoldsNullableHouseToHoldsHouse() => DeserializeSlow(HoldsNullableHouse, HoldsHouse);

    [Fact] public void NullableTuple2ToTuple2() => DeserializeSlow(NullableTuple2, Tuple2);
    [Fact] public void Tuple2ToNullableTuple2() => DeserializeSlow(Tuple2, NullableTuple2);
    [Fact] public void WithNullableTuple2ToWithTuple2() => DeserializeSlow(WithNullableTuple2, WithTuple2);
    [Fact] public void DefaultWithNullableTuple2ToWithTuple2() => DeserializeSlow(DefaultWithNullableTuple2, RealDefaultWithTuple2);
    [Fact] public void WithTuple2ToWithNullableTuple2() => DeserializeSlow(WithTuple2, WithNullableTuple2);

    [Fact] public void NullableTuple3ToTuple3() => DeserializeSlow(NullableTuple3, Tuple3);
    [Fact] public void Tuple3ToNullableTuple3() => DeserializeSlow(Tuple3, NullableTuple3);
    [Fact] public void WithNullableTuple3ToWithTuple3() => DeserializeSlow(WithNullableTuple3, WithTuple3);
    [Fact] public void WithTuple3ToWithNullableTuple3() => DeserializeSlow(WithTuple3, WithNullableTuple3);

    [Fact] public void WithDictionaryToWithNullableDictionary() => DeserializeFast(WithDictionary, WithNullableDictionary);
    [Fact] public void WithNullableDictionaryToWithDictionary() => DeserializeFast(WithNullableDictionary, WithDictionary);
    [Fact] public void DefaultWithNullableDictionaryToWithDictionary() => DeserializeFast(DefaultWithNullableDictionary, DefaultWithDictionaryNull);
    [Fact] public void DefaultWithNullableDictionaryToWithNullableDictionaryOfCats() => DeserializeSlow(DefaultWithNullableDictionary, DefaultWithNullableDictionaryOfCats);
}
