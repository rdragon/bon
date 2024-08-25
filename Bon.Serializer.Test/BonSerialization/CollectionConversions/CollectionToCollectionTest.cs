namespace Bon.Serializer.Test.BonSerialization.CollectionConversions;

public sealed class CollectionToCollectionTest : BonSerializerTestBase
{
    // A collection can be converted to another collection with a different element type.
    // Every element will be converted to the element type of the target collection.

    [Fact] public void ArrayToArrayOfCats() => DeserializeSlow(Array, ArrayOfCats);
    [Fact] public void OtherArrayToArrayOfCats() => DeserializeSlow(OtherArray, OtherArrayOfCats);
    [Fact] public void DefaultArrayToArrayOfCats() => DeserializeSlow(DefaultArray, DefaultArrayOfCats);
    [Fact] public void ArrayToIntArray() => DeserializeSlow(Array, new[] { 0, 0 });
    [Fact] public void IntArrayToStringArray() => DeserializeSlow(new[] { 5 }, new[] { "5" });
    [Fact] public void NullableIntArrayToStringArray() => DeserializeSlow(new int?[] { 5, null }, new[] { "5", "" });

    // A nullable array this is null is converted to an empty array if the target is non-nullable.

    [Fact] public void DefaultNullableArrayToArray() => DeserializeSlow(DefaultWithNullableArray, DefaultWithArrayOfEmptyClass);
    [Fact] public void DefaultNullableArrayToList() => DeserializeSlow(DefaultWithNullableArray, DefaultWithListOfEmptyClass);
    [Fact] public void DefaultNullableByteArrayToArray() => DeserializeSlow(DefaultWithNullableByteArray, DefaultWithByteArray);

    // A nullable array this is null remains null if the target is nullable.

    [Fact] public void DefaultNullableArrayToNullableArray() => DeserializeSlow(DefaultWithNullableArray, DefaultWithNullableArrayOfEmptyClass);
    [Fact] public void DefaultNullableArrayToNullableList() => DeserializeSlow(DefaultWithNullableArray, DefaultWithNullableListOfEmptyClass);

}
