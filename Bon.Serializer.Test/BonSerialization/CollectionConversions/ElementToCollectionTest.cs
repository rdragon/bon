namespace Bon.Serializer.Test.BonSerialization.CollectionConversions;

public sealed class ElementToCollectionTest : BonSerializerTestBase
{
    // A value X can be converted to a collection of size one containing X.

    [Fact] public void ElementToArray() => DeserializeSlow(Dog, OtherArray);
    [Fact] public void ElementToList() => DeserializeSlow(Dog, OtherList);
    [Fact] public void ElementToIList() => DeserializeSlow(Dog, OtherIList);
    [Fact] public void ElementToIReadOnlyList() => DeserializeSlow(Dog, OtherIReadOnlyList);
    [Fact] public void ElementToICollection() => DeserializeSlow(Dog, OtherICollection);
    [Fact] public void ElementToIReadOnlyCollection() => DeserializeSlow(Dog, OtherIReadOnlyCollection);
    [Fact] public void ElementToIEnumerable() => DeserializeSlow(Dog, OtherIEnumerable);

    [Fact] public void WithElementToWithArray() => DeserializeSlow(WithDog, OtherWithArray);
    [Fact] public void WithElementToWithList() => DeserializeSlow(WithDog, OtherWithList);
    [Fact] public void WithElementToWithIList() => DeserializeSlow(WithDog, OtherWithIList);
    [Fact] public void WithElementToWithIReadOnlyList() => DeserializeSlow(WithDog, OtherWithIReadOnlyList);
    [Fact] public void WithElementToWithICollection() => DeserializeSlow(WithDog, OtherWithICollection);
    [Fact] public void WithElementToWithIReadOnlyCollection() => DeserializeSlow(WithDog, OtherWithIReadOnlyCollection);
    [Fact] public void WithElementToWithIEnumerable() => DeserializeSlow(WithDog, OtherWithIEnumerable);

    [Fact] public void ElementToArrayOfNullable() => DeserializeSlow(House, new House?[] { House });
    [Fact] public void NullableElementToArray() => DeserializeSlow(NullableHouse, new[] { House });
    [Fact] public void NullableElementToArrayOfNullable() => DeserializeSlow(NullableHouse, new House?[] { House });

    [Fact] public void WithElementToWithArrayOfNullable() => DeserializeSlow(WithDog, OtherWithArrayOfNullable);
    [Fact] public void WithElementToWithNullableArray() => DeserializeSlow(WithDog, OtherWithNullableArray);

    [Fact] public void WithNullableElementToWithArray() => DeserializeSlow(WithNullableDog, OtherWithArray);
    [Fact] public void WithNullableElementToWithArrayOfNullable() => DeserializeSlow(WithNullableDog, OtherWithArrayOfNullable);
    [Fact] public void WithNullableElementToWithNullableArray() => DeserializeSlow(WithNullableDog, OtherWithNullableArray);

    // If X is null then it is converted to an empty collection.

    [Fact] public void DefaultNullableElementToArrayOfNullable() => DeserializeSlow(DefaultNullableHouse, System.Array.Empty<House?>());
    [Fact] public void DefaultWithNullableElementToWithArray() => DeserializeSlow(DefaultWithNullableDog, DefaultWithArray);
    [Fact] public void DefaultWithNullableElementToWithList() => DeserializeSlow(DefaultWithNullableDog, DefaultWithList);
    [Fact] public void DefaultWithNullableElementToWithArrayOfNullable() => DeserializeSlow(DefaultWithNullableDog, DefaultWithArrayOfNullable);

    // Except if the collection is nullable, then the result is also null.

    [Fact] public void DefaultWithNullableElementToWithNullableArray() => DeserializeSlow(DefaultWithNullableDog, DefaultWithNullableArray);
    [Fact] public void DefaultWithNullableElementToWithNullableList() => DeserializeSlow(DefaultWithNullableDog, DefaultWithNullableListOfEmptyClass);

    // However, there is one bug.
    // If the elements in the collection are of a non-nullable value type then null is converted to an collection of size one containing
    // the default value of the element type.
    // The reason for this is that the value (null in this case) is first converted to the element type (see bookmark 943797192).
    // Only if that result is null is an empty collection returned.
    // So if the element type is non-nullable, the result can never be null.
    // Therefore the result of that conversion is then put in an collection of size one.
    // This could be solved in the future.
    // Note that for reference types this bug does not exist.
    // If the element typs is a non-nullable reference type then the value is first converted to the nullable version of that reference type.

    [Fact] public void DefaultNullableElementToArray() => DeserializeSlow(DefaultNullableHouse, new[] { DefaultHouse });

    // The value can also be of a different type than the element type of the collection.
    // In that case the value is converted to the element type of the collection.

    [Fact] public void IntToStringArray() => DeserializeSlow(5, new[] { "5" });
    [Fact] public void StringToIntArray() => DeserializeSlow("5", new[] { 5 });
    [Fact] public void DogToCatArray() => DeserializeSlow(Dog, new[] { Cat });
    [Fact] public void IntArrayToStringArrayArray() => DeserializeSlow(new[] { 5 }, new[] { new[] { "5" } });

    // The conversion is recursive, so you can also go from a value to a collection of collections.

    [Fact] public void IntToStringArrayArray() => DeserializeSlow(5, new[] { new[] { "5" } });
}
