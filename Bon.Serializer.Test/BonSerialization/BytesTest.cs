namespace Bon.Serializer.Test.BonSerialization;

public sealed class BytesTest : BonSerializerTestBase
{
    // Integer types are serialized with their native number of bytes.

    [Fact]
    public void IntBytes() => GetManualSerializer()
        .WriteFullInt(Int)
        .ShouldBeBodyFor(Int);

    // Except if you serialize an integer directly and its small enough to fit in a smaller schema.

    [Fact]
    public void SmallIntBytes() => GetManualSerializer()
        .WriteByte((byte)SmallInt)
        .ShouldBeBodyFor(SmallInt);

    // Nullable integer types are serialized using a variable width schema.

    [Fact]
    public void NullableIntBytes() => GetManualSerializer()
        .WriteSignedWholeNumber(NullableInt)
        .ShouldBeBodyFor(NullableInt);

    // A struct does not take up more space than the space of its members.

    [Fact]
    public void HoldsIntBytes() => GetManualSerializer()
        .WriteFullInt(Int)
        .ShouldBeBodyFor(HoldsInt);

    [Fact]
    public void HoldsSmallIntBytes() => GetManualSerializer()
        .WriteFullInt(SmallInt)
        .ShouldBeBodyFor(HoldsSmallInt);

    // A class does not take up more space than the space of its members, except for a nullability byte at the start.
    // Also, if there are no members at all, an extra NULL byte is written.

    [Fact]
    public void EmptyClass_Body() => GetManualSerializer()
        .WriteNotNull()
        .WriteNull()
        .ShouldBeBodyFor(EmptyClass);

    [Fact]
    public void WithInt70_Body() => GetManualSerializer()
        .WriteNotNull()
        .WriteFullInt(70)
        .ShouldBeBodyFor(WithInt70);

    [Fact]
    public void NullClass_Body() => GetManualSerializer()
        .WriteNull()
        .ShouldBeBodyFor(NullWithInt);

    [Fact]
    public void WithDogBytes() => GetManualSerializer()
        .WriteNotNull()
        .WriteNotNull()
        .WriteFullInt(WithDog.Dog.Age)
        .ShouldBeBodyFor(WithDog);

    [Fact]
    public void WithNullableDogBytes() => RequireSameSerialization(WithDog, WithNullableDog);

    [Fact]
    public void DefaultWithNullableDogBytes() => GetManualSerializer()
        .WriteNotNull()
        .WriteNull()
        .ShouldBeBodyFor(DefaultWithNullableDog);

    // The members of a class are serialized by member ID ascending.

    [Fact]
    public void ThreeIntsScrambledBytes() => GetManualSerializer()
        .WriteNotNull()
        .WriteFullInt(ThreeIntsScrambled.Int1)
        .WriteFullInt(ThreeIntsScrambled.Int3)
        .WriteFullInt(ThreeIntsScrambled.Int2)
        .ShouldBeBodyFor(ThreeIntsScrambled);

    // Gaps between the member IDs do not affect the serialization.

    [Fact]
    public void TwoIntsWithGapBytes() => GetManualSerializer()
        .WriteNotNull()
        .WriteFullInt(TwoIntsWithGap.Int1)
        .WriteFullInt(TwoIntsWithGap.Int2)
        .ShouldBeBodyFor(TwoIntsWithGap);

    // A nullable struct is serialized exactly like a class.

    [Fact] public void NullableEmptyStructBytes() => RequireSameSerialization(EmptyClass, NullableEmptyStruct);
    [Fact] public void NullableHouseBytes() => RequireSameSerialization(Dog, NullableHouse);
    [Fact] public void NullableHoldsNullableHouseBytes() => RequireSameSerialization(WithDog, NullableHoldsNullableHouse);
    [Fact] public void DefaultHoldsNullableHouseBytes() => RequireSameSerialization(DefaultWithNullableDog, NullableDefaultHoldsNullableHouse);

    // An interface is serialized by writing the ID of the concrete type followed by the concrete type.

    [Fact]
    public void IDogBytes() => GetManualSerializer()
        .WriteCompactInt(DogId)
        .WriteFullInt(Dog.Age)
        .ShouldBeBodyFor(IDog);

    [Fact]
    public void WithIDogBytes() => GetManualSerializer()
        .WriteNotNull()
        .WriteCompactInt(DogId)
        .WriteFullInt(Dog.Age)
        .ShouldBeBodyFor(WithIDog);

    [Fact]
    public void RealDefaultIDogBytes() => GetManualSerializer()
        .WriteNull()
        .ShouldBeBodyFor(RealDefaultIDog);

    // An enum is serialized as its underlying type.

    [Fact] public void IntEnumBytes() => RequireSameSerialization((int)IntEnum, IntEnum, ForbidSchemaTypeOptimization);
    [Fact] public void ByteEnumBytes() => RequireSameSerialization((byte)ByteEnum, ByteEnum, ForbidSchemaTypeOptimization);
    [Fact] public void NullableIntEnumBytes() => RequireSameSerialization((int?)NullableIntEnum, NullableIntEnum);
    [Fact] public void NullableByteEnumBytes() => RequireSameSerialization((byte?)NullableByteEnum, NullableByteEnum);

    // An array is serialized by writing the element count followed by the elements.

    [Fact]
    public void IntArrayBytes() => GetManualSerializer()
        .WriteCompactInt(2)
        .WriteFullInt(IntArray[0])
        .WriteFullInt(IntArray[1])
        .ShouldBeBodyFor(IntArray);

    [Fact]
    public void RealDefaultIntArrayBytes() => GetManualSerializer()
        .WriteNull()
        .ShouldBeBodyFor(RealDefaultIntArray);

    [Fact]
    public void ByteArrayBytes() => GetManualSerializer()
        .WriteCompactInt(3)
        .WriteByte(ByteArray[0])
        .WriteByte(ByteArray[1])
        .WriteByte(ByteArray[2])
        .ShouldBeBodyFor(ByteArray);

    [Fact]
    public void WithArrayBytes() => GetManualSerializer()
        .WriteNotNull()
        .WriteCompactInt(2)
        .WriteNotNull()
        .WriteFullInt(WithArray[0].Age)
        .WriteNotNull()
        .WriteFullInt(WithArray[1].Age)
        .ShouldBeBodyFor(WithArray);

    [Fact]
    public void AnotherWithArrayOfNullableBytes() => RequireSameSerialization(WithArray, AnotherWithArrayOfNullable);

    [Fact]
    public void WithArrayOfEmptyClassBytes() => GetManualSerializer()
        .WriteNotNull()
        .WriteCompactInt(2)
        .WriteNotNull()
        .WriteNull()
        .WriteNotNull()
        .WriteNull()
        .ShouldBeBodyFor(WithArrayOfEmptyClass);

    [Fact]
    public void DefaultWithNullableArrayOfDogsBytes() => GetManualSerializer()
        .WriteNotNull()
        .WriteNull()
        .ShouldBeBodyFor(DefaultWithNullableArray);

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
    public void DictionaryBytes() => GetManualSerializer()
        .WriteCompactInt(1)
        .WriteFullInt(Int)
        .WriteNotNull()
        .WriteFullInt(Dog.Age)
        .ShouldBeBodyFor(Dictionary);

    [Fact]
    public void RealDefaultDictionaryBytes() => GetManualSerializer()
        .WriteNull()
        .ShouldBeBodyFor(RealDefaultDictionary);

    [Fact]
    public void WithDictionaryBytes() => GetManualSerializer()
        .WriteNotNull()
        .WriteCompactInt(1)
        .WriteFullInt(Int)
        .WriteNotNull()
        .WriteFullInt(Dog.Age)
        .ShouldBeBodyFor(WithDictionary);

    // A 2-tuple is serialized by writing the two elements.

    [Fact]
    public void IntTuple2Bytes() => GetManualSerializer()
        .WriteFullInt(Int)
        .WriteFullInt(OtherInt)
        .ShouldBeBodyFor(IntTuple2);

    [Fact]
    public void NullableIntTuple2Bytes() => GetManualSerializer()
        .WriteNotNull()
        .WriteFullInt(Int)
        .WriteFullInt(OtherInt)
        .ShouldBeBodyFor(NullableIntTuple2);

    [Fact]
    public void DefaultNullableIntTuple2Bytes() => GetManualSerializer()
        .WriteNull()
        .ShouldBeBodyFor(DefaultNullableIntTuple2);

    [Fact]
    public void WithTuple2Bytes() => GetManualSerializer()
        .WriteNotNull()
        .WriteNotNull()
        .WriteFullInt(Dog.Age)
        .WriteFullInt(Int)
        .ShouldBeBodyFor(WithTuple2);

    [Fact]
    public void WithNullableTuple2Bytes() => GetManualSerializer()
        .WriteNotNull()
        .WriteNotNull()
        .WriteNotNull()
        .WriteFullInt(Dog.Age)
        .WriteFullInt(Int)
        .ShouldBeBodyFor(WithNullableTuple2);

    // byte?, ushort?, uint?, ulong?, char? are serialized in the same way.

    [Fact] public void NullableByteBytes() => RequireSameSerialization((ulong?)NullableByte, NullableByte);
    [Fact] public void NullableUShortBytes() => RequireSameSerialization((ulong?)NullableUShort, NullableUShort);
    [Fact] public void NullableUIntBytes() => RequireSameSerialization((ulong?)NullableUInt, NullableUInt);
    [Fact] public void NullableCharBytes() => RequireSameSerialization((ulong?)NullableChar, NullableChar);

    // sbyte?, short?, int?, long? are serialized in the same way.

    [Fact] public void NullableSByteBytes() => RequireSameSerialization((long?)NullableSByte, NullableSByte);
    [Fact] public void NullableShortBytes() => RequireSameSerialization((long?)NullableShort, NullableShort);
    [Fact] public void NullableIntBytes1() => RequireSameSerialization((long?)NullableInt, NullableInt);

    // DateTime, DateTimeOffset, TimeSpan, TimeOnly are serialized as long. DateOnly is serialized as int.

    [Fact] public void DateTimeBytes() => RequireSameSerialization(TestHelper.DateTime.ToUniversalTime().Ticks, TestHelper.DateTime, ForbidSchemaTypeOptimization);
    [Fact] public void DateTimeOffsetBytes() => RequireSameSerialization(TestHelper.DateTimeOffset.UtcTicks, TestHelper.DateTimeOffset, ForbidSchemaTypeOptimization);
    [Fact] public void TimeSpanBytes() => RequireSameSerialization(TestHelper.TimeSpan.Ticks, TestHelper.TimeSpan, ForbidSchemaTypeOptimization);
    [Fact] public void DateOnlyBytes() => RequireSameSerialization(TestHelper.DateOnly.DayNumber, TestHelper.DateOnly, ForbidSchemaTypeOptimization);
    [Fact] public void TimeOnlyBytes() => RequireSameSerialization(TestHelper.TimeOnly.Ticks, TestHelper.TimeOnly, ForbidSchemaTypeOptimization);

    // If you serialize an integer type directly, its schema is implicit.

    [Fact]
    public void ByteMessage() => GetManualSerializer()
        .WriteByteMessage(Byte)
        .ShouldBeMessageFor(Byte);

    [Fact]
    public void UShortMessage() => GetManualSerializer()
        .WriteUShortMessage(UShort)
        .ShouldBeMessageFor(UShort);

    [Fact]
    public void UIntMessage() => GetManualSerializer()
        .WriteUIntMessage(UInt)
        .ShouldBeMessageFor(UInt);

    [Fact]
    public void ULongMessage() => GetManualSerializer()
        .WriteULongMessage(ULong)
        .ShouldBeMessageFor(ULong);

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
        .ShouldBeMessageFor(value);

    [Theory]
    [InlineData(short.MaxValue)]
    [InlineData(short.MinValue)]
    public void ShortMessage(short value) => GetManualSerializer()
        .WriteShortMessage(value)
        .ShouldBeMessageFor(value);

    [Theory]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void IntMessage(int value) => GetManualSerializer()
        .WriteIntMessage(value)
        .ShouldBeMessageFor(value);

    [Theory]
    [InlineData(long.MaxValue)]
    [InlineData(long.MinValue)]
    public void LongMessage(long value) => GetManualSerializer()
        .WriteLongMessage(value)
        .ShouldBeMessageFor(value);

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
