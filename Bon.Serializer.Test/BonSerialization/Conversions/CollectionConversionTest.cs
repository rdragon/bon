namespace Bon.Serializer.Test.BonSerialization.Conversions;

public sealed class CollectionConversionTest : BonSerializerTestBase
{
    // A collection can be converted to another collection with a different element type.
    // Every element will be converted to the element type of the target collection.

    [Fact] public void ArrayToArrayOfCats() => DeserializeSlow(Array, ArrayOfCats);
    [Fact] public void OtherArrayToArrayOfCats() => DeserializeSlow(OtherArray, OtherArrayOfCats);
    [Fact] public void DefaultArrayToArrayOfCats() => DeserializeSlow(DefaultArray, DefaultArrayOfCats);
    [Fact] public void ArrayToIntArray() => DeserializeSlow(Array, new[] { 0, 0 });
    [Fact] public void IntArrayToStringArray() => DeserializeSlow(new[] { 5 }, new[] { "5" });
    [Fact] public void NullableIntArrayToStringArray() => DeserializeSlow(new int?[] { 5, null }, new[] { "5", null });
    [Fact] public void NullableStringArrayToIntArray() => DeserializeSlow(new string?[] { "5", null }, new[] { 5, 0 });
}
