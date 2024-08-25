namespace Bon.Serializer.Test.BonSerialization.CollectionConversions;

public sealed class CollectionToElementTest : BonSerializerTestBase
{
    // A collection of size one containing X can be converted to X.
    // It is enough to only check the array collection type because all collection types are serialized to the same bytes.

    [Fact] public void OtherArrayToElement() => DeserializeSlow(OtherArray, Dog);
    [Fact] public void OtherWithArrayToWithElement() => DeserializeSlow(OtherWithArray, WithDog);
    [Fact] public void ArrayOfNullableToElement() => DeserializeSlow(new House?[] { House }, House);
    [Fact] public void WithArrayOfNullableToWithElement() => DeserializeSlow(OtherWithArrayOfNullable, WithDog);

    // The same thing happens when the target type is nullable.

    [Fact] public void ArrayToNullableElement() => DeserializeSlow(new[] { House }, NullableHouse);
    [Fact] public void ArrayOfNullableToNullableElement() => DeserializeSlow(new House?[] { House }, NullableHouse);
    [Fact] public void OtherWithArrayToNullableElement() => DeserializeSlow(OtherWithArray, WithNullableDog);
    [Fact] public void OtherWithArrayOfNullableToNullableElement() => DeserializeSlow(OtherWithArrayOfNullable, WithNullableDog);

    // A collection of more than one element can be converted to the first element.

    [Fact] public void ArrayToElement() => DeserializeSlow(Array, Dog);

    // The value can also be of a different type than the element type of the collection.

    [Fact] public void IntArrayToString() => DeserializeSlow(new[] { 5 }, "5");
    [Fact] public void StringArrayToInt() => DeserializeSlow(new[] { "5" }, 5);
    [Fact] public void DogArrayToCat() => DeserializeSlow(new[] { Dog }, Cat);
    [Fact] public void IntArrayArrayToStringArray() => DeserializeSlow(new[] { new[] { 5 } }, new[] { "5" });

    // The conversion is recursive, so you can also convert a collection of collections to an element.

    [Fact] public void IntArrayArrayToString() => DeserializeSlow(new[] { new[] { 5 } }, "5");

    // An empty collection is converted to the default value of the target type.

    [Fact] public void DefaultArrayToElement() => DeserializeSlow(DefaultArray, DefaultDog);
    [Fact] public void DefaultArrayToNullableElement() => DeserializeSlow(DefaultWithArray, DefaultWithNullableDog);

    // A nullable collection that is null is also converted to the default value of the target type.

    [Fact] public void DefaultNullableArrayToElement() => DeserializeSlow(DefaultWithNullableArray, DefaultWithDog);
    [Fact] public void DefaultNullableArrayToNullableElement() => DeserializeSlow(DefaultWithNullableArray, DefaultWithNullableDog);
}
