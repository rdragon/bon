namespace Bon.Serializer.Test.BonSerialization;

public sealed class BytesTest : BonSerializerTestBase
{
    // A class does not take up more space than the space of its members.

    [Fact]
    public void EmptyClassBytes() => GetSimpleSerializer()
        .WriteClassHeader<EmptyClass>()
        .ShouldEqual(EmptyClass);

    [Fact]
    public void DogBytes() => GetSimpleSerializer()
        .WriteClassHeader<Dog>()
        .WriteInt(Dog.Age)
        .ShouldEqual(Dog);

    [Fact]
    public void WithDogBytes() => GetSimpleSerializer()
        .WriteClassHeader<WithDog>()
        .WriteInt(Dog.Age)
        .ShouldEqual(WithDog);

    [Fact]
    public void WithWithDogBytes() => GetSimpleSerializer()
        .WriteClassHeader<WithWithDog>()
        .WriteInt(Dog.Age)
        .ShouldEqual(WithWithDog);

    // A nullable class takes up one more byte. However, if the class is null it takes up only one byte.

    [Fact]
    public void WithNullableDogBytes() => GetSimpleSerializer()
        .WriteClassHeader<WithNullableDog>()
        .WriteByte(NOT_NULL)
        .WriteInt(WithDog.Dog.Age)
        .ShouldEqual(WithNullableDog);

    [Fact]
    public void DefaultWithNullableDogBytes() => GetSimpleSerializer()
        .WriteClassHeader<WithNullableDog>()
        .WriteByte(NULL)
        .ShouldEqual(DefaultWithNullableDog);

    // The members of a class are serialized by member ID ascending.

    [Fact]
    public void ThreeIntsScrambledBytes() => GetSimpleSerializer()
        .WriteClassHeader<ThreeIntsScrambled>()
        .WriteInt(ThreeIntsScrambled.Int1)
        .WriteInt(ThreeIntsScrambled.Int3)
        .WriteInt(ThreeIntsScrambled.Int2)
        .ShouldEqual(ThreeIntsScrambled);

    // Gaps between the member IDs do not affect the serialization.

    [Fact]
    public void TwoIntsWithGapBytes() => GetSimpleSerializer()
        .WriteClassHeader<TwoIntsWithGap>()
        .WriteInt(TwoIntsWithGap.Int1)
        .WriteInt(TwoIntsWithGap.Int2)
        .ShouldEqual(TwoIntsWithGap);

    // A struct is serialized exactly like a class.

    [Fact] public void EmptyStructBytes() => RequireSameSerialization(EmptyClass, EmptyStruct);
    [Fact] public void HouseBytes() => RequireSameSerialization(Dog, House);
    [Fact] public void HoldsHouseBytes() => RequireSameSerialization(WithDog, HoldsHouse);
    [Fact] public void HoldsNullableHouseBytes() => RequireSameSerialization(WithNullableDog, HoldsNullableHouse);
    [Fact] public void DefaultHoldsNullableHouseBytes() => RequireSameSerialization(DefaultWithNullableDog, DefaultHoldsNullableHouse);

    // An interface is serialized by writing the ID of the concrete type followed by the concrete type.

    [Fact]
    public void IDogBytes() => GetSimpleSerializer()
        .WriteInterfaceHeader<IDog>()
        .WriteWholeNumber(DogId)
        .WriteInt(Dog.Age)
        .ShouldEqual(IDog);

    [Fact]
    public void WithIDogBytes() => GetSimpleSerializer()
        .WriteClassHeader<WithIDog>()
        .WriteWholeNumber(DogId)
        .WriteInt(Dog.Age)
        .ShouldEqual(WithIDog);

    // An enum is serialized as its underlying type.

    [Fact] public void IntEnumBytes() => RequireSameSerialization((int)IntEnum, IntEnum);
    [Fact] public void ByteEnumBytes() => RequireSameSerialization((byte)ByteEnum, ByteEnum);
    [Fact] public void NullableIntEnumBytes() => RequireSameSerialization((int?)NullableIntEnum, NullableIntEnum);
    [Fact] public void NullableByteEnumBytes() => RequireSameSerialization((byte?)NullableByteEnum, NullableByteEnum);

    // An array is serialized by writing the element count followed by the elements.

    [Fact]
    public void WithArrayBytes() => GetSimpleSerializer()
        .WriteClassHeader<WithArray>()
        .WriteWholeNumber(2)
        .WriteInt(WithArray[0].Age)
        .WriteInt(WithArray[1].Age)
        .ShouldEqual(WithArray);

    [Fact]
    public void WithArrayOfNullableBytes() => GetSimpleSerializer()
        .WriteClassHeader<WithArrayOfNullable>()
        .WriteWholeNumber(2)
        .WriteByte(NOT_NULL)
        .WriteInt(WithArray[0].Age)
        .WriteByte(NULL)
        .ShouldEqual(WithArrayOfNullable);

    [Fact]
    public void WithArrayOfEmptyClassBytes() => GetSimpleSerializer()
        .WriteClassHeader<WithArrayOfEmptyClass>()
        .WriteWholeNumber(2)
        .ShouldEqual(WithArrayOfEmptyClass);

    // A nullable array that is null takes up one byte.

    [Fact]
    public void DefaultWithNullableArrayOfDogsBytes() => GetSimpleSerializer()
        .WriteClassHeader<WithNullableArray>()
        .WriteWholeNumber(null)
        .ShouldEqual(DefaultWithNullableArray);

    // All collection types are serialized exactly the same way.

    [Fact] public void ListBytes() => RequireSameSerialization(Array, List);
    [Fact] public void IListBytes() => RequireSameSerialization(Array, IList);
    [Fact] public void IReadOnlyListBytes() => RequireSameSerialization(Array, IReadOnlyList);
    [Fact] public void ICollectionBytes() => RequireSameSerialization(Array, ICollection);
    [Fact] public void IReadOnlyCollectionBytes() => RequireSameSerialization(Array, IReadOnlyCollection);
    [Fact] public void IEnumerableBytes() => RequireSameSerialization(Array, IEnumerable);

    [Fact] public void WithListBytes() => RequireSameSerialization(WithArray, WithList);
    [Fact] public void WithIListBytes() => RequireSameSerialization(WithArray, WithIList);
    [Fact] public void WithIReadOnlyListBytes() => RequireSameSerialization(WithArray, WithIReadOnlyList);
    [Fact] public void WithICollectionBytes() => RequireSameSerialization(WithArray, WithICollection);
    [Fact] public void WithIReadOnlyCollectionBytes() => RequireSameSerialization(WithArray, WithIReadOnlyCollection);
    [Fact] public void WithIEnumerableBytes() => RequireSameSerialization(WithArray, WithIEnumerable);

    // A dictionary is serialized by writing the element count followed by the elements.

    [Fact]
    public void WithDictionaryBytes() => GetSimpleSerializer()
        .WriteClassHeader<WithDictionary>()
        .WriteWholeNumber(1)
        .WriteInt(Int)
        .WriteInt(Dog.Age)
        .ShouldEqual(WithDictionary);

    // A 2-tuple is serialized by writing the two elements.

    [Fact]
    public void WithTuple2Bytes() => GetSimpleSerializer()
        .WriteClassHeader<WithTuple2>()
        .WriteInt(Dog.Age)
        .WriteInt(Int)
        .ShouldEqual(WithTuple2);

    [Fact]
    public void WithNullableTuple2Bytes() => GetSimpleSerializer()
        .WriteClassHeader<WithNullableTuple2>()
        .WriteByte(NOT_NULL)
        .WriteInt(Dog.Age)
        .WriteInt(Int)
        .ShouldEqual(WithNullableTuple2);

    // byte?, ushort?, uint?, ulong?, char? are serialized in the same way.

    [Fact] public void NullableByteBytes() => RequireSameSerialization((ulong?)NullableByte, NullableByte);
    [Fact] public void NullableUShortBytes() => RequireSameSerialization((ulong?)NullableUShort, NullableUShort);
    [Fact] public void NullableUIntBytes() => RequireSameSerialization((ulong?)NullableUInt, NullableUInt);
    [Fact] public void NullableCharBytes() => RequireSameSerialization((ulong?)NullableChar, NullableChar);

    // sbyte?, short?, int?, long? are serialized in the same way.

    [Fact] public void NullableSByteBytes() => RequireSameSerialization((long?)NullableSByte, NullableSByte);
    [Fact] public void NullableShortBytes() => RequireSameSerialization((long?)NullableShort, NullableShort);
    [Fact] public void NullableIntBytes() => RequireSameSerialization((long?)NullableInt, NullableInt);

    // DateTime, DateTimeOffset, TimeSpan, TimeOnly are serialized as long. DateOnly is serialized as int.

    [Fact] public void DateTimeBytes() => RequireSameSerialization(TestHelper.DateTime.ToUniversalTime().Ticks, TestHelper.DateTime);
    [Fact] public void DateTimeOffsetBytes() => RequireSameSerialization(TestHelper.DateTimeOffset.UtcTicks, TestHelper.DateTimeOffset);
    [Fact] public void TimeSpanBytes() => RequireSameSerialization(TestHelper.TimeSpan.Ticks, TestHelper.TimeSpan);
    [Fact] public void DateOnlyBytes() => RequireSameSerialization(TestHelper.DateOnly.DayNumber, TestHelper.DateOnly);
    [Fact] public void TimeOnlyBytes() => RequireSameSerialization(TestHelper.TimeOnly.Ticks, TestHelper.TimeOnly);
}
