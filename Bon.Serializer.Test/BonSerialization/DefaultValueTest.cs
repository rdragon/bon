namespace Bon.Serializer.Test.BonSerialization;

public sealed class DefaultValueTest : BonSerializerTestBase
{
    // Keep in sync with the file at bookmark 451785766.

    [Fact] public void EmptyClassDefaultValue() => TestDefaultValue(EmptyClass);
    [Fact] public void EmptyStructDefaultValue() => TestDefaultValue(EmptyStruct);

    [Fact] public void IAnimalDefaultValue() => TestDefaultValue(DefaultIAnimal);
    [Fact] public void IntEnumDefaultValue() => TestDefaultValue(DefaultIntEnum);
    [Fact] public void ByteEnumDefaultValue() => TestDefaultValue(DefaultByteEnum);

    [Fact] public void ArrayDefaultValue() => TestDefaultValue(DefaultArray);
    [Fact] public void ListDefaultValue() => TestDefaultValue(DefaultList);
    [Fact] public void IReadOnlyListDefaultValue() => TestDefaultValue(DefaultIReadOnlyList);
    [Fact] public void IListDefaultValue() => TestDefaultValue(DefaultIList);
    [Fact] public void IReadOnlyCollectionDefaultValue() => TestDefaultValue(DefaultIReadOnlyCollection);
    [Fact] public void ICollectionDefaultValue() => TestDefaultValue(DefaultICollection);
    [Fact] public void IEnumerableDefaultValue() => TestDefaultValue(DefaultIEnumerable);

    [Fact] public void WithDogDefaultValue() => TestDefaultValue(DefaultWithDog);
    [Fact] public void WithNullableDogDefaultValue() => TestDefaultValue(DefaultWithNullableDog);

    [Fact] public void WithIDogDefaultValue() => TestDefaultValue(DefaultWithIDog);
    [Fact] public void WithNullableIDogDefaultValue() => TestDefaultValue(DefaultWithNullableIDog);

    [Fact] public void WithDayOfWeekDefaultValue() => TestDefaultValue(DefaultWithDayOfWeek);
    [Fact] public void WithNullableDayOfWeekDefaultValue() => TestDefaultValue(DefaultWithNullableDayOfWeek);

    [Fact] public void WithArrayDefaultValue() => TestDefaultValue(DefaultWithArray);
    [Fact] public void WithListDefaultValue() => TestDefaultValue(DefaultWithList);
    [Fact] public void WithIListDefaultValue() => TestDefaultValue(DefaultWithIList);
    [Fact] public void WithIReadOnlyListDefaultValue() => TestDefaultValue(DefaultWithIReadOnlyList);
    [Fact] public void WithICollectionDefaultValue() => TestDefaultValue(DefaultWithICollection);
    [Fact] public void WithIReadOnlyCollectionDefaultValue() => TestDefaultValue(DefaultWithIReadOnlyCollection);
    [Fact] public void WithIEnumerableDefaultValue() => TestDefaultValue(DefaultWithIEnumerable);

    [Fact] public void StringDefaultValue() => TestDefaultValue(DefaultString);
    [Fact] public void BoolDefaultValue() => TestDefaultValue(DefaultBool);
    [Fact] public void NullableBoolDefaultValue() => TestDefaultValue(DefaultNullableBool);
    [Fact] public void ByteDefaultValue() => TestDefaultValue(DefaultByte);
    [Fact] public void NullableByteDefaultValue() => TestDefaultValue(DefaultNullableByte);
    [Fact] public void SByteDefaultValue() => TestDefaultValue(DefaultSByte);
    [Fact] public void NullableSByteDefaultValue() => TestDefaultValue(DefaultNullableSByte);
    [Fact] public void ShortDefaultValue() => TestDefaultValue(DefaultShort);
    [Fact] public void NullableShortDefaultValue() => TestDefaultValue(DefaultNullableShort);
    [Fact] public void UShortDefaultValue() => TestDefaultValue(DefaultUShort);
    [Fact] public void NullableUShortDefaultValue() => TestDefaultValue(DefaultNullableUShort);
    [Fact] public void IntDefaultValue() => TestDefaultValue(DefaultInt);
    [Fact] public void NullableIntDefaultValue() => TestDefaultValue(DefaultNullableInt);
    [Fact] public void UIntDefaultValue() => TestDefaultValue(DefaultUInt);
    [Fact] public void NullableUIntDefaultValue() => TestDefaultValue(DefaultNullableUInt);
    [Fact] public void LongDefaultValue() => TestDefaultValue(DefaultLong);
    [Fact] public void NullableLongDefaultValue() => TestDefaultValue(DefaultNullableLong);
    [Fact] public void ULongDefaultValue() => TestDefaultValue(DefaultULong);
    [Fact] public void NullableULongDefaultValue() => TestDefaultValue(DefaultNullableULong);
    [Fact] public void FloatDefaultValue() => TestDefaultValue(DefaultFloat);
    [Fact] public void NullableFloatDefaultValue() => TestDefaultValue(DefaultNullableFloat);
    [Fact] public void DoubleDefaultValue() => TestDefaultValue(DefaultDouble);
    [Fact] public void NullableDoubleDefaultValue() => TestDefaultValue(DefaultNullableDouble);
    [Fact] public void DecimalDefaultValue() => TestDefaultValue(DefaultDecimal);
    [Fact] public void NullableDecimalDefaultValue() => TestDefaultValue(DefaultNullableDecimal);
    [Fact] public void GuidDefaultValue() => TestDefaultValue(DefaultGuid);
    [Fact] public void NullableGuidDefaultValue() => TestDefaultValue(DefaultNullableGuid);

    [Fact] public void WithStringDefaultValue() => TestDefaultValue(DefaultWithString);
    [Fact] public void WithNullableStringDefaultValue() => TestDefaultValue(DefaultWithNullableString);
    [Fact] public void WithBoolDefaultValue() => TestDefaultValue(DefaultWithBool);
    [Fact] public void WithNullableBoolDefaultValue() => TestDefaultValue(DefaultWithNullableBool);
    [Fact] public void WithByteDefaultValue() => TestDefaultValue(DefaultWithByte);
    [Fact] public void WithNullableByteDefaultValue() => TestDefaultValue(DefaultWithNullableByte);
    [Fact] public void WithSByteDefaultValue() => TestDefaultValue(DefaultWithSByte);
    [Fact] public void WithNullableSByteDefaultValue() => TestDefaultValue(DefaultWithNullableSByte);
    [Fact] public void WithShortDefaultValue() => TestDefaultValue(DefaultWithShort);
    [Fact] public void WithNullableShortDefaultValue() => TestDefaultValue(DefaultWithNullableShort);
    [Fact] public void WithUShortDefaultValue() => TestDefaultValue(DefaultWithUShort);
    [Fact] public void WithNullableUShortDefaultValue() => TestDefaultValue(DefaultWithNullableUShort);
    [Fact] public void WithIntDefaultValue() => TestDefaultValue(DefaultWithInt);
    [Fact] public void WithNullableIntDefaultValue() => TestDefaultValue(DefaultWithNullableInt);
    [Fact] public void WithUIntDefaultValue() => TestDefaultValue(DefaultWithUInt);
    [Fact] public void WithNullableUIntDefaultValue() => TestDefaultValue(DefaultWithNullableUInt);
    [Fact] public void WithLongDefaultValue() => TestDefaultValue(DefaultWithLong);
    [Fact] public void WithNullableLongDefaultValue() => TestDefaultValue(DefaultWithNullableLong);
    [Fact] public void WithULongDefaultValue() => TestDefaultValue(DefaultWithULong);
    [Fact] public void WithNullableULongDefaultValue() => TestDefaultValue(DefaultWithNullableULong);
    [Fact] public void WithFloatDefaultValue() => TestDefaultValue(DefaultWithFloat);
    [Fact] public void WithNullableFloatDefaultValue() => TestDefaultValue(DefaultWithNullableFloat);
    [Fact] public void WithDoubleDefaultValue() => TestDefaultValue(DefaultWithDouble);
    [Fact] public void WithNullableDoubleDefaultValue() => TestDefaultValue(DefaultWithNullableDouble);
    [Fact] public void WithDecimalDefaultValue() => TestDefaultValue(DefaultWithDecimal);
    [Fact] public void WithNullableDecimalDefaultValue() => TestDefaultValue(DefaultWithNullableDecimal);
    [Fact] public void WithGuidDefaultValue() => TestDefaultValue(DefaultWithGuid);
    [Fact] public void WithNullableGuidDefaultValue() => TestDefaultValue(DefaultWithNullableGuid);

    [Fact] public void HouseDefaultValue() => TestDefaultValue(DefaultHouse);
    [Fact] public void HoldsHouseDefaultValue() => TestDefaultValue(DefaultHoldsHouse);
    [Fact] public void HoldsNullableHouseDefaultValue() => TestDefaultValue(DefaultHoldsNullableHouse);

    [Fact] public void WithArrayOfNullableDefaultValue() => TestDefaultValue(DefaultWithArrayOfNullable);

    [Fact] public void DateTimeDefaultValue() => TestDefaultValue(DefaultDateTime);
    [Fact] public void NullableDateTimeDefaultValue() => TestDefaultValue(DefaultNullableDateTime);
    [Fact] public void WithDateTimeDefaultValue() => TestDefaultValue(DefaultWithDateTime);
    [Fact] public void WithNullableDateTimeDefaultValue() => TestDefaultValue(DefaultWithNullableDateTime);

    [Fact] public void DateTimeOffsetDefaultValue() => TestDefaultValue(DefaultDateTimeOffset);
    [Fact] public void NullableDateTimeOffsetDefaultValue() => TestDefaultValue(DefaultNullableDateTimeOffset);
    [Fact] public void WithDateTimeOffsetDefaultValue() => TestDefaultValue(DefaultWithDateTimeOffset);
    [Fact] public void WithNullableDateTimeOffsetDefaultValue() => TestDefaultValue(DefaultWithNullableDateTimeOffset);

    [Fact] public void DateOnlyDefaultValue() => TestDefaultValue(DefaultDateOnly);
    [Fact] public void NullableDateOnlyDefaultValue() => TestDefaultValue(DefaultNullableDateOnly);
    [Fact] public void WithDateOnlyDefaultValue() => TestDefaultValue(DefaultWithDateOnly);
    [Fact] public void WithNullableDateOnlyDefaultValue() => TestDefaultValue(DefaultWithNullableDateOnly);

    [Fact] public void TimeOnlyDefaultValue() => TestDefaultValue(DefaultTimeOnly);
    [Fact] public void NullableTimeOnlyDefaultValue() => TestDefaultValue(DefaultNullableTimeOnly);
    [Fact] public void WithTimeOnlyDefaultValue() => TestDefaultValue(DefaultWithTimeOnly);
    [Fact] public void WithNullableTimeOnlyDefaultValue() => TestDefaultValue(DefaultWithNullableTimeOnly);

    [Fact] public void TimeSpanDefaultValue() => TestDefaultValue(DefaultTimeSpan);
    [Fact] public void NullableTimeSpanDefaultValue() => TestDefaultValue(DefaultNullableTimeSpan);
    [Fact] public void WithTimeSpanDefaultValue() => TestDefaultValue(DefaultWithTimeSpan);
    [Fact] public void WithNullableTimeSpanDefaultValue() => TestDefaultValue(DefaultWithNullableTimeSpan);

    [Fact] public void DictionaryDefaultValue() => TestDefaultValue(DefaultDictionary);
    [Fact] public void IDictionaryDefaultValue() => TestDefaultValue(DefaultIDictionary);
    [Fact] public void IReadOnlyDictionaryDefaultValue() => TestDefaultValue(DefaultIReadOnlyDictionary);
    [Fact] public void WithDictionaryDefaultValue() => TestDefaultValue(DefaultWithDictionary);
    [Fact] public void WithIDictionaryDefaultValue() => TestDefaultValue(DefaultWithIDictionary);
    [Fact] public void WithIReadOnlyDictionaryDefaultValue() => TestDefaultValue(DefaultWithIReadOnlyDictionary);
    [Fact] public void WithNullableDictionaryDefaultValue() => TestDefaultValue(DefaultWithNullableDictionary);

    [Fact] public void Tuple2DefaultValue() => TestDefaultValue(DefaultTuple2);
    [Fact] public void NullableTuple2DefaultValue() => TestDefaultValue(DefaultNullableTuple2);
    [Fact] public void WithTuple2DefaultValue() => TestDefaultValue(DefaultWithTuple2);
    [Fact] public void WithNullableTuple2DefaultValue() => TestDefaultValue(DefaultWithNullableTuple2);
    [Fact] public void WithTuple2OfNullablesDefaultValue() => TestDefaultValue(DefaultWithTuple2OfNullables);

    [Fact] public void Tuple3DefaultValue() => TestDefaultValue(DefaultTuple3);
    [Fact] public void NullableTuple3DefaultValue() => TestDefaultValue(DefaultNullableTuple3);
    [Fact] public void WithTuple3DefaultValue() => TestDefaultValue(DefaultWithTuple3);
    [Fact] public void WithNullableTuple3DefaultValue() => TestDefaultValue(DefaultWithNullableTuple3);
    [Fact] public void WithTuple3OfNullablesDefaultValue() => TestDefaultValue(DefaultWithTuple3OfNullables);

    [Fact] public void HoldsTwoIntsDefaultValue() => TestDefaultValue(DefaultHoldsTwoInts);

    [Fact] public void WithGenericClassDefaultValue() => TestDefaultValue(new WithGenericClass(new GenericClass<int>(0)));

    [Fact] public void WithFieldDefaultValue() => TestDefaultValue(DefaultWithField);

    [Fact] public void ByteArrayDefaultValue() => TestDefaultValue(DefaultByteArray);
    [Fact] public void WithByteArrayDefaultValue() => TestDefaultValue(DefaultWithByteArray);
    [Fact] public void WithNullableByteArrayDefaultValue() => TestDefaultValue(DefaultWithNullableByteArray);

    // ----------------------------------------------------------------------------------------------
    // End of the "keep in sync" region.
    // ----------------------------------------------------------------------------------------------

    [Fact] public void TurtleDefaultValue() => TestDefaultValue(DefaultTurtle);
    [Fact] public void INodeDefaultValue() => TestDefaultValue(DefaultINode);

    [Fact]
    public void DefaultListsAreUnique()
    {
        var list1 = GetDefaultValue<List<Dog>>();
        var list2 = GetDefaultValue<List<Dog>>();

        Assert.False(ReferenceEquals(list1, list2));
    }

    [Fact]
    public void DefaultListsAreUnique2()
    {
        var list1 = GetDefaultValue<WithList>().RawDeserializedCollection;
        var list2 = GetDefaultValue<WithList>().RawDeserializedCollection;

        Assert.False(ReferenceEquals(list1, list2));
    }

    [Fact]
    public void DefaultArraysAreTheSame()
    {
        var array1 = GetDefaultValue<Dog[]>();
        var array2 = GetDefaultValue<IReadOnlyList<Dog>>();
        var array3 = GetDefaultValue<IReadOnlyCollection<Dog>>();
        var array4 = GetDefaultValue<IEnumerable<Dog>>();

        Assert.True(ReferenceEquals(array1, array2));
        Assert.True(ReferenceEquals(array1, array3));
        Assert.True(ReferenceEquals(array1, array4));
    }

    [Fact]
    public void DefaultArraysAreTheSame2()
    {
        var array1 = GetDefaultValue<WithArray>().RawDeserializedCollection;
        var array2 = GetDefaultValue<WithIReadOnlyList>().RawDeserializedCollection;
        var array3 = GetDefaultValue<WithIReadOnlyCollection>().RawDeserializedCollection;
        var array4 = GetDefaultValue<WithIEnumerable>().RawDeserializedCollection;

        Assert.True(ReferenceEquals(array1, array2));
        Assert.True(ReferenceEquals(array1, array3));
        Assert.True(ReferenceEquals(array1, array4));
    }

    [Fact]
    public void DefaultIReadOnlyDictionariesAreTheSame()
    {
        var dictionary1 = GetDefaultValue<IReadOnlyDictionary<int, Dog>>();
        var dictionary2 = GetDefaultValue<IReadOnlyDictionary<int, Dog>>();

        Assert.True(ReferenceEquals(dictionary1, dictionary2));
    }

    [Fact]
    public void DefaultIReadOnlyDictionariesAreTheSame2()
    {
        var dictionary1 = GetDefaultValue<WithIReadOnlyDictionary>().RawDeserializedCollection;
        var dictionary2 = GetDefaultValue<WithIReadOnlyDictionary>().RawDeserializedCollection;

        Assert.True(ReferenceEquals(dictionary1, dictionary2));
    }

    [Fact]
    public void DefaultDictionariesAreUnique()
    {
        var dictionary1 = GetDefaultValue<Dictionary<int, Dog>>();
        var dictionary2 = GetDefaultValue<Dictionary<int, Dog>>();

        Assert.False(ReferenceEquals(dictionary1, dictionary2));
    }

    [Fact]
    public void DefaultDictionariesAreUnique2()
    {
        var dictionary1 = GetDefaultValue<WithDictionary>().RawDeserializedCollection;
        var dictionary2 = GetDefaultValue<WithDictionary>().RawDeserializedCollection;

        Assert.False(ReferenceEquals(dictionary1, dictionary2));
    }

    [Fact]
    public void DefaultIDictionariesAreUnique()
    {
        var dictionary1 = GetDefaultValue<IDictionary<int, Dog>>();
        var dictionary2 = GetDefaultValue<IDictionary<int, Dog>>();

        Assert.False(ReferenceEquals(dictionary1, dictionary2));
    }

    [Fact]
    public void DefaultIDictionariesAreUnique2()
    {
        var dictionary1 = GetDefaultValue<WithIDictionary>().RawDeserializedCollection;
        var dictionary2 = GetDefaultValue<WithIDictionary>().RawDeserializedCollection;

        Assert.False(ReferenceEquals(dictionary1, dictionary2));
    }

    [Fact] public void DefaultIListIsAList() => Assert.IsType<List<Dog>>(GetDefaultValue<IList<Dog>>());
    [Fact] public void DefaultIListIsAList2() => Assert.IsType<List<Dog>>(GetDefaultValue<WithIList>().RawDeserializedCollection);
    [Fact] public void DefaultIReadOnlyListIsAnArray() => Assert.IsType<Dog[]>(GetDefaultValue<IReadOnlyList<Dog>>());
    [Fact] public void DefaultIReadOnlyListIsAnArray2() => Assert.IsType<Dog[]>(GetDefaultValue<WithIReadOnlyList>().RawDeserializedCollection);
    [Fact] public void DefaultICollectionIsAList() => Assert.IsType<List<Dog>>(GetDefaultValue<ICollection<Dog>>());
    [Fact] public void DefaultICollectionIsAList2() => Assert.IsType<List<Dog>>(GetDefaultValue<WithICollection>().RawDeserializedCollection);
    [Fact] public void DefaultIReadOnlyCollectionIsAnArray() => Assert.IsType<Dog[]>(GetDefaultValue<IReadOnlyCollection<Dog>>());
    [Fact] public void DefaultIReadOnlyCollectionIsAnArray2() => Assert.IsType<Dog[]>(GetDefaultValue<WithIReadOnlyCollection>().RawDeserializedCollection);
    [Fact] public void DefaultIEnumerableIsAnArray() => Assert.IsType<Dog[]>(GetDefaultValue<IEnumerable<Dog>>());
    [Fact] public void DefaultIEnumerableIsAnArray2() => Assert.IsType<Dog[]>(GetDefaultValue<WithIEnumerable>().RawDeserializedCollection);
}
