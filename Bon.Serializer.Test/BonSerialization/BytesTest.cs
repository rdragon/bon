namespace Bon.Serializer.Test.BonSerialization;

public sealed class BytesTest : BonSerializerTestBase
{
    // A class does not take up more space than the space of its members.

    [Fact]
    public void EmptyClassBytes() => GetManualSerializer()
        .WriteClassHeader<EmptyClass>()
        .ShouldEqual(EmptyClass);

    [Fact]
    public void DogBytes() => GetManualSerializer()
        .WriteClassHeader<Dog>()
        .WriteInt(Dog.Age)
        .ShouldEqual(Dog);

    [Fact]
    public void WithDogBytes() => GetManualSerializer()
        .WriteClassHeader<WithDog>()
        .WriteInt(Dog.Age)
        .ShouldEqual(WithDog);

    [Fact]
    public void WithWithDogBytes() => GetManualSerializer()
        .WriteClassHeader<WithWithDog>()
        .WriteInt(Dog.Age)
        .ShouldEqual(WithWithDog);

    // A nullable class takes up one more byte. However, if the class is null it takes up only one byte.

    [Fact]
    public void WithNullableDogBytes() => GetManualSerializer()
        .WriteClassHeader<WithNullableDog>()
        .WriteByte(NOT_NULL)
        .WriteInt(WithDog.Dog.Age)
        .ShouldEqual(WithNullableDog);

    [Fact]
    public void DefaultWithNullableDogBytes() => GetManualSerializer()
        .WriteClassHeader<WithNullableDog>()
        .WriteByte(NULL)
        .ShouldEqual(DefaultWithNullableDog);

    // The members of a class are serialized by member ID ascending.

    [Fact]
    public void ThreeIntsScrambledBytes() => GetManualSerializer()
        .WriteClassHeader<ThreeIntsScrambled>()
        .WriteInt(ThreeIntsScrambled.Int1)
        .WriteInt(ThreeIntsScrambled.Int3)
        .WriteInt(ThreeIntsScrambled.Int2)
        .ShouldEqual(ThreeIntsScrambled);

    // Gaps between the member IDs do not affect the serialization.

    [Fact]
    public void TwoIntsWithGapBytes() => GetManualSerializer()
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
    public void IDogBytes() => GetManualSerializer()
        .WriteInterfaceHeader<IDog>()
        .WriteWholeNumber(DogId)
        .WriteInt(Dog.Age)
        .ShouldEqual(IDog);

    [Fact]
    public void WithIDogBytes() => GetManualSerializer()
        .WriteClassHeader<WithIDog>()
        .WriteWholeNumber(DogId)
        .WriteInt(Dog.Age)
        .ShouldEqual(WithIDog);

    // An enum is serialized as its underlying type.

    [Fact] public void IntEnumBytes() => RequireSameSerialization((int)IntEnum, IntEnum, ForbidSchemaTypeOptimization);
    [Fact] public void ByteEnumBytes() => RequireSameSerialization((byte)ByteEnum, ByteEnum, ForbidSchemaTypeOptimization);
    [Fact] public void NullableIntEnumBytes() => RequireSameSerialization((int?)NullableIntEnum, NullableIntEnum);
    [Fact] public void NullableByteEnumBytes() => RequireSameSerialization((byte?)NullableByteEnum, NullableByteEnum);

    // An array is serialized by writing the element count followed by the elements.

    [Fact]
    public void WithArrayBytes() => GetManualSerializer()
        .WriteClassHeader<WithArray>()
        .WriteWholeNumber(2)
        .WriteInt(WithArray[0].Age)
        .WriteInt(WithArray[1].Age)
        .ShouldEqual(WithArray);

    [Fact]
    public void WithArrayOfNullableBytes() => GetManualSerializer()
        .WriteClassHeader<WithArrayOfNullable>()
        .WriteWholeNumber(2)
        .WriteByte(NOT_NULL)
        .WriteInt(WithArray[0].Age)
        .WriteByte(NULL)
        .ShouldEqual(WithArrayOfNullable);

    [Fact]
    public void WithArrayOfEmptyClassBytes() => GetManualSerializer()
        .WriteClassHeader<WithArrayOfEmptyClass>()
        .WriteWholeNumber(2)
        .ShouldEqual(WithArrayOfEmptyClass);

    // A nullable array that is null takes up one byte.

    [Fact]
    public void DefaultWithNullableArrayOfDogsBytes() => GetManualSerializer()
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
    public void WithDictionaryBytes() => GetManualSerializer()
        .WriteClassHeader<WithDictionary>()
        .WriteWholeNumber(1)
        .WriteInt(Int)
        .WriteInt(Dog.Age)
        .ShouldEqual(WithDictionary);

    // A 2-tuple is serialized by writing the two elements.

    [Fact]
    public void WithTuple2Bytes() => GetManualSerializer()
        .WriteClassHeader<WithTuple2>()
        .WriteInt(Dog.Age)
        .WriteInt(Int)
        .ShouldEqual(WithTuple2);

    [Fact]
    public void WithNullableTuple2Bytes() => GetManualSerializer()
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

    [Fact] public void DateTimeBytes() => RequireSameSerialization(TestHelper.DateTime.ToUniversalTime().Ticks, TestHelper.DateTime, ForbidSchemaTypeOptimization);
    [Fact] public void DateTimeOffsetBytes() => RequireSameSerialization(TestHelper.DateTimeOffset.UtcTicks, TestHelper.DateTimeOffset, ForbidSchemaTypeOptimization);
    [Fact] public void TimeSpanBytes() => RequireSameSerialization(TestHelper.TimeSpan.Ticks, TestHelper.TimeSpan, ForbidSchemaTypeOptimization);
    [Fact] public void DateOnlyBytes() => RequireSameSerialization(TestHelper.DateOnly.DayNumber, TestHelper.DateOnly, ForbidSchemaTypeOptimization);
    [Fact] public void TimeOnlyBytes() => RequireSameSerialization(TestHelper.TimeOnly.Ticks, TestHelper.TimeOnly, ForbidSchemaTypeOptimization);

    // The integer types are serialized in an optimized way.

    [Fact]
    public void ByteMessage() => GetManualSerializer()
        .WriteByteMessage(Byte)
        .ShouldEqual(Byte);

    [Fact]
    public void UShortMessage() => GetManualSerializer()
        .WriteUShortMessage(UShort)
        .ShouldEqual(UShort);

    [Fact]
    public void UIntMessage() => GetManualSerializer()
        .WriteUIntMessage(UInt)
        .ShouldEqual(UInt);

    [Fact]
    public void ULongMessage() => GetManualSerializer()
        .WriteULongMessage(ULong)
        .ShouldEqual(ULong);

    // If an integer is small enough then it is serialized using a smaller schema.

    [Fact] public void UShortAsByte() => RequireSameSerialization(Byte, (ushort)Byte);
    [Fact] public void UIntAsByte() => RequireSameSerialization(Byte, (uint)Byte);
    [Fact] public void ULongAsByte() => RequireSameSerialization(Byte, (ulong)Byte);
    [Fact] public void UIntAsUShort() => RequireSameSerialization(UShort, (uint)UShort);
    [Fact] public void ULongAsUShort() => RequireSameSerialization(UShort, (ulong)UShort);
    [Fact] public void ULongAsUInt() => RequireSameSerialization(UInt, (ulong)UInt);

    [Theory]
    [InlineData(sbyte.MaxValue)]
    [InlineData(sbyte.MinValue)]
    public void SByteMessage(sbyte value) => GetManualSerializer()
        .WriteSByteMessage(value)
        .ShouldEqual(value);

    [Theory]
    [InlineData(short.MaxValue)]
    [InlineData(short.MinValue)]
    public void ShortMessage(short value) => GetManualSerializer()
        .WriteShortMessage(value)
        .ShouldEqual(value);

    [Theory]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void IntMessage(int value) => GetManualSerializer()
        .WriteIntMessage(value)
        .ShouldEqual(value);

    [Theory]
    [InlineData(long.MaxValue)]
    [InlineData(long.MinValue)]
    public void LongMessage(long value) => GetManualSerializer()
        .WriteLongMessage(value)
        .ShouldEqual(value);

    [Fact] public void ShortAsByte() => RequireSameSerialization(Byte, (short)Byte);
    [Fact] public void IntAsByte() => RequireSameSerialization(Byte, (int)Byte);
    [Fact] public void LongAsByte() => RequireSameSerialization(Byte, (long)Byte);
    [Fact] public void IntAsShort() => RequireSameSerialization(Short, (int)Short);
    [Fact] public void LongAsShort() => RequireSameSerialization(Short, (long)Short);
    [Fact] public void LongAsInt() => RequireSameSerialization(Int, (long)Int);

    [Fact] public void NegativeShortAsSByte() => RequireSameSerialization(NegativeSByte, (short)NegativeSByte);
    [Fact] public void NegativeIntAsSByte() => RequireSameSerialization(NegativeSByte, (int)NegativeSByte);
    [Fact] public void NegativeLongAsSByte() => RequireSameSerialization(NegativeSByte, (long)NegativeSByte);
    [Fact] public void NegativeIntAsShort() => RequireSameSerialization(NegativeShort, (int)NegativeShort);
    [Fact] public void NegativeLongAsShort() => RequireSameSerialization(NegativeShort, (long)NegativeShort);
    [Fact] public void NegativeLongAsInt() => RequireSameSerialization(NegativeInt, (long)NegativeInt);
}
