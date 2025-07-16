namespace Bon.Serializer.Test.BonSerialization;

public sealed class RoundTripTest : BonSerializerTestBase
{
    [Fact] public void EmptyClassRoundTrip() => RoundTripFast(EmptyClass);
    [Fact] public void EmptyStructRoundTrip() => RoundTripFast(EmptyStruct);

    [Fact] public void IAnimalRoundTrip() => RoundTripFast(IAnimal);
    [Fact] public void OtherIAnimalRoundTrip() => RoundTripFast(OtherIAnimal);

    [Fact] public void IntEnumRoundTrip() => RoundTripFast(IntEnum);
    [Fact] public void NullableIntEnumRoundTrip() => RoundTripFast(NullableIntEnum);
    [Fact] public void DefaultNullableIntEnumRoundTrip() => RoundTripFast(DefaultNullableIntEnum);

    [Fact] public void ByteEnumRoundTrip() => RoundTripFast(ByteEnum);
    [Fact] public void NullableByteEnumRoundTrip() => RoundTripFast(NullableByteEnum);
    [Fact] public void DefaultNullableByteEnumRoundTrip() => RoundTripFast(DefaultNullableByteEnum);

    [Fact] public void ArrayRoundTrip() => RoundTripSlow(Array);
    [Fact] public void ListRoundTrip() => RoundTripSlow(List);
    [Fact] public void IReadOnlyListRoundTrip() => RoundTripSlow(IReadOnlyList);
    [Fact] public void IListRoundTrip() => RoundTripSlow(IList);
    [Fact] public void IReadOnlyCollectionRoundTrip() => RoundTripSlow(IReadOnlyCollection);
    [Fact] public void ICollectionRoundTrip() => RoundTripSlow(ICollection);
    [Fact] public void IEnumerableRoundTrip() => RoundTripSlow(IEnumerable);

    [Fact] public void WithDogRoundTrip() => RoundTripFast(WithDog);
    [Fact] public void WithNullableDogRoundTrip() => RoundTripFast(WithNullableDog);
    [Fact] public void DefaultWithNullableDogRoundTrip() => RoundTripFast(DefaultWithNullableDog);

    [Fact] public void WithIDogRoundTrip() => RoundTripFast(WithIDog);
    [Fact] public void WithNullableIDogRoundTrip() => RoundTripFast(WithNullableIDog);
    [Fact] public void DefaultWithNullableIDogRoundTrip() => RoundTripFast(DefaultWithNullableIDog);

    [Fact] public void WithDayOfWeekRoundTrip() => RoundTripFast(WithDayOfWeek);
    [Fact] public void WithNullableDayOfWeekRoundTrip() => RoundTripFast(WithNullableDayOfWeek);
    [Fact] public void DefaultWithNullableDayOfWeekRoundTrip() => RoundTripFast(DefaultWithNullableDayOfWeek);

    [Fact] public void WithArrayRoundTrip() => RoundTripFast(WithArray);
    [Fact] public void WithEmptyArrayRoundTrip() => RoundTripFast(DefaultWithArray);
    [Fact] public void WithSingleElementArrayRoundTrip() => RoundTripFast(OtherWithArray);
    [Fact] public void WithNullableArrayRoundTrip() => RoundTripFast(WithNullableArray);
    [Fact] public void DefaultWithNullableArrayRoundTrip() => RoundTripFast(DefaultWithNullableArray);

    [Fact] public void WithListRoundTrip() => RoundTripFast(WithList);
    [Fact] public void WithIListRoundTrip() => RoundTripFast(WithIList);
    [Fact] public void WithIReadOnlyListRoundTrip() => RoundTripFast(WithIReadOnlyList);
    [Fact] public void WithICollectionRoundTrip() => RoundTripFast(WithICollection);
    [Fact] public void WithIReadOnlyCollectionRoundTrip() => RoundTripFast(WithIReadOnlyCollection);
    [Fact] public void WithIEnumerableRoundTrip() => RoundTripFast(WithIEnumerable);

    [Fact] public void StringRoundTrip() => RoundTripFast(String);
    [Fact] public void BoolRoundTrip() => RoundTripSlow(Bool);
    [Fact] public void NullableBoolRoundTrip() => RoundTripSlow(NullableBool);
    [Fact] public void DefaultNullableBoolRoundTrip() => RoundTripSlow(DefaultNullableBool);
    [Fact] public void ByteRoundTrip() => RoundTripFast(Byte);
    [Fact] public void NullableByteRoundTrip() => RoundTripSlow(NullableByte);
    [Fact] public void DefaultNullableByteRoundTrip() => RoundTripSlow(DefaultNullableByte);
    [Fact] public void SByteRoundTrip() => RoundTripFast(SByte);
    [Fact] public void NullableSByteRoundTrip() => RoundTripSlow(NullableSByte);
    [Fact] public void DefaultNullableSByteRoundTrip() => RoundTripSlow(DefaultNullableSByte);
    [Fact] public void ShortRoundTrip() => RoundTripFast(Short);
    [Fact] public void NullableShortRoundTrip() => RoundTripSlow(NullableShort);
    [Fact] public void DefaultNullableShortRoundTrip() => RoundTripSlow(DefaultNullableShort);
    [Fact] public void UShortRoundTrip() => RoundTripFast(UShort);
    [Fact] public void NullableUShortRoundTrip() => RoundTripSlow(NullableUShort);
    [Fact] public void DefaultNullableUShortRoundTrip() => RoundTripSlow(DefaultNullableUShort);
    [Fact] public void IntRoundTrip() => RoundTripFast(Int);
    [Fact] public void NullableIntRoundTrip() => RoundTripSlow(NullableInt);
    [Fact] public void DefaultNullableIntRoundTrip() => RoundTripSlow(DefaultNullableInt);
    [Fact] public void UIntRoundTrip() => RoundTripFast(UInt);
    [Fact] public void NullableUIntRoundTrip() => RoundTripSlow(NullableUInt);
    [Fact] public void DefaultNullableUIntRoundTrip() => RoundTripSlow(DefaultNullableUInt);
    [Fact] public void LongRoundTrip() => RoundTripFast(Long);
    [Fact] public void NullableLongRoundTrip() => RoundTripFast(NullableLong);
    [Fact] public void DefaultNullableLongRoundTrip() => RoundTripFast(DefaultNullableLong);
    [Fact] public void ULongRoundTrip() => RoundTripFast(ULong);
    [Fact] public void NullableULongRoundTrip() => RoundTripFast(NullableULong);
    [Fact] public void DefaultNullableULongRoundTrip() => RoundTripFast(DefaultNullableULong);
    [Fact] public void FloatRoundTrip() => RoundTripFast(Float);
    [Fact] public void NullableFloatRoundTrip() => RoundTripSlow(NullableFloat);
    [Fact] public void DefaultNullableFloatRoundTrip() => RoundTripSlow(DefaultNullableFloat);
    [Fact] public void DoubleRoundTrip() => RoundTripFast(Double);
    [Fact] public void NullableDoubleRoundTrip() => RoundTripFast(NullableDouble);
    [Fact] public void DefaultNullableDoubleRoundTrip() => RoundTripFast(DefaultNullableDouble);
    [Fact] public void DecimalRoundTrip() => RoundTripSlow(Decimal);
    [Fact] public void NullableDecimalRoundTrip() => RoundTripFast(NullableDecimal);
    [Fact] public void DefaultNullableDecimalRoundTrip() => RoundTripFast(DefaultNullableDecimal);
    [Fact] public void GuidRoundTrip() => RoundTripSlow(Guid);
    [Fact] public void NullableGuidRoundTrip() => RoundTripSlow(NullableGuid);
    [Fact] public void DefaultNullableGuidRoundTrip() => RoundTripSlow(DefaultNullableGuid);

    [Fact] public void WithStringRoundTrip() => RoundTripFast(WithString);
    [Fact] public void DefaultWithStringRoundTrip() => RoundTripFast(DefaultWithString);
    [Fact] public void WithNullableStringRoundTrip() => RoundTripFast(WithNullableString);
    [Fact] public void DefaultWithNullableStringRoundTrip() => RoundTripFast(DefaultWithNullableString);
    [Fact] public void WithBoolRoundTrip() => RoundTripFast(WithBool);
    [Fact] public void DefaultWithBoolRoundTrip() => RoundTripFast(DefaultWithBool);
    [Fact] public void WithNullableBoolRoundTrip() => RoundTripFast(WithNullableBool);
    [Fact] public void DefaultWithNullableBoolRoundTrip() => RoundTripFast(DefaultWithNullableBool);
    [Fact] public void WithByteRoundTrip() => RoundTripFast(WithByte);
    [Fact] public void DefaultWithByteRoundTrip() => RoundTripFast(DefaultWithByte);
    [Fact] public void WithNullableByteRoundTrip() => RoundTripFast(WithNullableByte);
    [Fact] public void DefaultWithNullableByteRoundTrip() => RoundTripFast(DefaultWithNullableByte);
    [Fact] public void WithSByteRoundTrip() => RoundTripFast(WithSByte);
    [Fact] public void DefaultWithSByteRoundTrip() => RoundTripFast(DefaultWithSByte);
    [Fact] public void WithNullableSByteRoundTrip() => RoundTripFast(WithNullableSByte);
    [Fact] public void DefaultWithNullableSByteRoundTrip() => RoundTripFast(DefaultWithNullableSByte);
    [Fact] public void WithShortRoundTrip() => RoundTripFast(WithShort);
    [Fact] public void DefaultWithShortRoundTrip() => RoundTripFast(DefaultWithShort);
    [Fact] public void WithNullableShortRoundTrip() => RoundTripFast(WithNullableShort);
    [Fact] public void DefaultWithNullableShortRoundTrip() => RoundTripFast(DefaultWithNullableShort);
    [Fact] public void WithUShortRoundTrip() => RoundTripFast(WithUShort);
    [Fact] public void DefaultWithUShortRoundTrip() => RoundTripFast(DefaultWithUShort);
    [Fact] public void WithNullableUShortRoundTrip() => RoundTripFast(WithNullableUShort);
    [Fact] public void DefaultWithNullableUShortRoundTrip() => RoundTripFast(DefaultWithNullableUShort);
    [Fact] public void WithIntRoundTrip() => RoundTripFast(WithInt);
    [Fact] public void DefaultWithIntRoundTrip() => RoundTripFast(DefaultWithInt);
    [Fact] public void WithNullableIntRoundTrip() => RoundTripFast(WithNullableInt);
    [Fact] public void DefaultWithNullableIntRoundTrip() => RoundTripFast(DefaultWithNullableInt);
    [Fact] public void WithUIntRoundTrip() => RoundTripFast(WithUInt);
    [Fact] public void DefaultWithUIntRoundTrip() => RoundTripFast(DefaultWithUInt);
    [Fact] public void WithNullableUIntRoundTrip() => RoundTripFast(WithNullableUInt);
    [Fact] public void DefaultWithNullableUIntRoundTrip() => RoundTripFast(DefaultWithNullableUInt);
    [Fact] public void WithLongRoundTrip() => RoundTripFast(WithLong);
    [Fact] public void DefaultWithLongRoundTrip() => RoundTripFast(DefaultWithLong);
    [Fact] public void WithNullableLongRoundTrip() => RoundTripFast(WithNullableLong);
    [Fact] public void DefaultWithNullableLongRoundTrip() => RoundTripFast(DefaultWithNullableLong);
    [Fact] public void WithULongRoundTrip() => RoundTripFast(WithULong);
    [Fact] public void DefaultWithULongRoundTrip() => RoundTripFast(DefaultWithULong);
    [Fact] public void WithNullableULongRoundTrip() => RoundTripFast(WithNullableULong);
    [Fact] public void DefaultWithNullableULongRoundTrip() => RoundTripFast(DefaultWithNullableULong);
    [Fact] public void WithFloatRoundTrip() => RoundTripFast(WithFloat);
    [Fact] public void DefaultWithFloatRoundTrip() => RoundTripFast(DefaultWithFloat);
    [Fact] public void WithNullableFloatRoundTrip() => RoundTripFast(WithNullableFloat);
    [Fact] public void DefaultWithNullableFloatRoundTrip() => RoundTripFast(DefaultWithNullableFloat);
    [Fact] public void WithDoubleRoundTrip() => RoundTripFast(WithDouble);
    [Fact] public void DefaultWithDoubleRoundTrip() => RoundTripFast(DefaultWithDouble);
    [Fact] public void WithNullableDoubleRoundTrip() => RoundTripFast(WithNullableDouble);
    [Fact] public void DefaultWithNullableDoubleRoundTrip() => RoundTripFast(DefaultWithNullableDouble);
    [Fact] public void WithDecimalRoundTrip() => RoundTripFast(WithDecimal);
    [Fact] public void DefaultWithDecimalRoundTrip() => RoundTripFast(DefaultWithDecimal);
    [Fact] public void WithNullableDecimalRoundTrip() => RoundTripFast(WithNullableDecimal);
    [Fact] public void DefaultWithNullableDecimalRoundTrip() => RoundTripFast(DefaultWithNullableDecimal);
    [Fact] public void WithGuidRoundTrip() => RoundTripFast(WithGuid);
    [Fact] public void DefaultWithGuidRoundTrip() => RoundTripFast(DefaultWithGuid);
    [Fact] public void WithNullableGuidRoundTrip() => RoundTripFast(WithNullableGuid);
    [Fact] public void DefaultWithNullableGuidRoundTrip() => RoundTripFast(DefaultWithNullableGuid);

    [Fact] public void HouseRoundTrip() => RoundTripFast(House);
    [Fact] public void HoldsHouseRoundTrip() => RoundTripFast(HoldsHouse);
    [Fact] public void HoldsNullableHouseRoundTrip() => RoundTripFast(HoldsNullableHouse);

    [Fact] public void WithArrayOfNullableRoundTrip() => RoundTripFast(WithArrayOfNullable);

    [Fact] public void DateTimeRoundTrip() => RoundTripSlow(TestHelper.DateTime);
    [Fact] public void NullableDateTimeRoundTrip() => RoundTripSlow(NullableDateTime);
    [Fact] public void DefaultNullableDateTimeRoundTrip() => RoundTripSlow(DefaultNullableDateTime);
    [Fact] public void WithDateTimeRoundTrip() => RoundTripFast(WithDateTime);
    [Fact] public void DefaultWithDateTimeRoundTrip() => RoundTripFast(DefaultWithDateTime);
    [Fact] public void WithNullableDateTimeRoundTrip() => RoundTripFast(WithNullableDateTime);
    [Fact] public void DefaultWithNullableDateTimeRoundTrip() => RoundTripFast(DefaultWithNullableDateTime);

    [Fact] public void DateTimeOffsetRoundTrip() => RoundTripSlow(TestHelper.DateTimeOffset);
    [Fact] public void NullableDateTimeOffsetRoundTrip() => RoundTripSlow(NullableDateTimeOffset);
    [Fact] public void DefaultNullableDateTimeOffsetRoundTrip() => RoundTripSlow(DefaultNullableDateTimeOffset);
    [Fact] public void WithDateTimeOffsetRoundTrip() => RoundTripFast(WithDateTimeOffset);
    [Fact] public void DefaultWithDateTimeOffsetRoundTrip() => RoundTripFast(DefaultWithDateTimeOffset);
    [Fact] public void WithNullableDateTimeOffsetRoundTrip() => RoundTripFast(WithNullableDateTimeOffset);
    [Fact] public void DefaultWithNullableDateTimeOffsetRoundTrip() => RoundTripFast(DefaultWithNullableDateTimeOffset);

    [Fact] public void DateOnlyRoundTrip() => RoundTripSlow(TestHelper.DateOnly);
    [Fact] public void NullableDateOnlyRoundTrip() => RoundTripSlow(NullableDateOnly);
    [Fact] public void DefaultNullableDateOnlyRoundTrip() => RoundTripSlow(DefaultNullableDateOnly);
    [Fact] public void WithDateOnlyRoundTrip() => RoundTripFast(WithDateOnly);
    [Fact] public void DefaultWithDateOnlyRoundTrip() => RoundTripFast(DefaultWithDateOnly);
    [Fact] public void WithNullableDateOnlyRoundTrip() => RoundTripFast(WithNullableDateOnly);
    [Fact] public void DefaultWithNullableDateOnlyRoundTrip() => RoundTripFast(DefaultWithNullableDateOnly);

    [Fact] public void TimeOnlyRoundTrip() => RoundTripSlow(TestHelper.TimeOnly);
    [Fact] public void NullableTimeOnlyRoundTrip() => RoundTripSlow(NullableTimeOnly);
    [Fact] public void DefaultNullableTimeOnlyRoundTrip() => RoundTripSlow(DefaultNullableTimeOnly);
    [Fact] public void WithTimeOnlyRoundTrip() => RoundTripFast(WithTimeOnly);
    [Fact] public void DefaultWithTimeOnlyRoundTrip() => RoundTripFast(DefaultWithTimeOnly);
    [Fact] public void WithNullableTimeOnlyRoundTrip() => RoundTripFast(WithNullableTimeOnly);
    [Fact] public void DefaultWithNullableTimeOnlyRoundTrip() => RoundTripFast(DefaultWithNullableTimeOnly);

    [Fact] public void TimeSpanRoundTrip() => RoundTripSlow(TestHelper.TimeSpan);
    [Fact] public void NullableTimeSpanRoundTrip() => RoundTripSlow(NullableTimeSpan);
    [Fact] public void DefaultNullableTimeSpanRoundTrip() => RoundTripSlow(DefaultNullableTimeSpan);
    [Fact] public void WithTimeSpanRoundTrip() => RoundTripFast(WithTimeSpan);
    [Fact] public void DefaultWithTimeSpanRoundTrip() => RoundTripFast(DefaultWithTimeSpan);
    [Fact] public void WithNullableTimeSpanRoundTrip() => RoundTripFast(WithNullableTimeSpan);
    [Fact] public void DefaultWithNullableTimeSpanRoundTrip() => RoundTripFast(DefaultWithNullableTimeSpan);

    [Fact] public void DictionaryRoundTrip() => RoundTripSlow(Dictionary);
    [Fact] public void IDictionaryRoundTrip() => RoundTripSlow(IDictionary);
    [Fact] public void IReadOnlyDictionaryRoundTrip() => RoundTripSlow(IReadOnlyDictionary);
    [Fact] public void WithDictionaryRoundTrip() => RoundTripFast(WithDictionary);
    [Fact] public void WithIDictionaryRoundTrip() => RoundTripFast(WithIDictionary);
    [Fact] public void WithIReadOnlyDictionaryRoundTrip() => RoundTripFast(WithIReadOnlyDictionary);
    [Fact] public void WithNullableDictionaryRoundTrip() => RoundTripFast(WithNullableDictionary);

    [Fact] public void Tuple2RoundTrip() => RoundTripSlow(Tuple2);
    [Fact] public void NullableTuple2RoundTrip() => RoundTripSlow(NullableTuple2);
    [Fact] public void WithTuple2RoundTrip() => RoundTripFast(WithTuple2);
    [Fact] public void WithNullableTuple2RoundTrip() => RoundTripFast(WithNullableTuple2);
    [Fact] public void WithTuple2OfNullablesRoundTrip() => RoundTripFast(WithTuple2OfNullables);

    [Fact] public void Tuple3RoundTrip() => RoundTripSlow(Tuple3);
    [Fact] public void NullableTuple3RoundTrip() => RoundTripSlow(NullableTuple3);
    [Fact] public void WithTuple3RoundTrip() => RoundTripFast(WithTuple3);
    [Fact] public void WithNullableTuple3RoundTrip() => RoundTripFast(WithNullableTuple3);
    [Fact] public void WithTuple3OfNullablesRoundTrip() => RoundTripFast(WithTuple3OfNullables);

    [Fact] public void HoldsTwoIntsRoundTrip() => RoundTripFast(HoldsTwoInts);

    [Fact] public void WithGenericClassRoundTrip() => RoundTripFast(new WithGenericClass(new GenericClass<int>(5)));

    [Fact] public void WithFieldRoundTrip() => RoundTripFast(WithField);

    [Fact] public void ByteArrayRoundTrip() => RoundTripSlow(ByteArray);
    [Fact] public void WithByteArrayRoundTrip() => RoundTripFast(WithByteArray);
    [Fact] public void WithNullableByteArrayRoundTrip() => RoundTripFast(WithNullableByteArray);
    [Fact] public void DefaultWithNullableByteArrayRoundTrip() => RoundTripFast(DefaultWithNullableByteArray);

    [Fact] public void DefaultNullableTuple2RoundTrip() => RoundTripSlow(DefaultNullableTuple2);
    [Fact] public void DefaultWithTuple2RoundTrip() => RoundTripFast(DefaultWithTuple2);

    [Fact] public void DefaultNullableTuple3RoundTrip() => RoundTripSlow(DefaultNullableTuple3);
    [Fact] public void DefaultWithTuple3RoundTrip() => RoundTripFast(DefaultWithTuple3);

    [Fact] public void DefaultWithDictionaryRoundTrip() => RoundTripFast(DefaultWithDictionary);
    [Fact] public void DictionaryOfCatsRoundTrip() => RoundTripSlow(DictionaryOfCats);

    [Fact] public void TwoIntsWithGapRoundTrip() => RoundTripFast(TwoIntsWithGap);
    [Fact] public void ThreeIntsScrambledRoundTrip() => RoundTripFast(ThreeIntsScrambled);

    [Fact] public void CharArrayRoundTrip() => RoundTripSlow(CharArray);
    [Fact] public void EnumerableRoundTrip() => RoundTripSlow(TestHelper.GetEnumerable());

    [Fact] public void ParentClassRoundTrip() => RoundTripFast((ParentClass)new ChildClass(5));

    [Fact] public void Turtle1RoundTrip() => RoundTripFast(Turtle1);
    [Fact] public void Turtle2RoundTrip() => RoundTripFast(Turtle2);
    [Fact] public void Turtle3RoundTrip() => RoundTripFast(Turtle3);
    [Fact] public void Tortoise1RoundTrip() => RoundTripFast(Tortoise1);
    [Fact] public void Tortoise2RoundTrip() => RoundTripFast(Tortoise2);
    [Fact] public void Tortoise3RoundTrip() => RoundTripFast(Tortoise3);

    [Fact] public void Node1RoundTrip() => RoundTripFast(Node1);
    [Fact] public void Node2RoundTrip() => RoundTripFast(Node2);
    [Fact] public void Node3RoundTrip() => RoundTripFast(Node3);
}
