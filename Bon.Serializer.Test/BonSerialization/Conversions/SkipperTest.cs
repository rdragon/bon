namespace Bon.Serializer.Test.BonSerialization.Conversions;

public sealed class SkipperTest : BonSerializerTestBase
{
    [Fact] public void SkipArray() => DeserializeSlow(WithArray, 0);
    [Fact] public void SkipClass() => DeserializeSlow(WithDog, 0);
    [Fact] public void SkipInterface() => DeserializeSlow(WithIDog, 0);
    [Fact] public void SkipTuple2() => DeserializeSlow(WithTuple2, 0);
    [Fact] public void SkipTuple3() => DeserializeSlow(WithTuple3, 0);
    [Fact] public void SkipDictionary() => DeserializeSlow(WithDictionary, 0);

    [Fact] public void SkipNullableArray() => DeserializeSlow(WithNullableArray, 0);
    [Fact] public void SkipNullableClass() => DeserializeSlow(WithNullableDog, 0);
    [Fact] public void SkipNullableInterface() => DeserializeSlow(WithNullableIDog, 0);
    [Fact] public void SkipNullableTuple2() => DeserializeSlow(WithNullableTuple2, 0);
    [Fact] public void SkipNullableTuple3() => DeserializeSlow(WithNullableTuple3, 0);
    [Fact] public void SkipNullableDictionary() => DeserializeSlow(WithNullableDictionary, 0);

    [Fact] public void SkipDefaultNullableArray() => DeserializeSlow(DefaultWithNullableArray, 0);
    [Fact] public void SkipDefaultNullableClass() => DeserializeSlow(DefaultWithNullableDog, 0);
    [Fact] public void SkipDefaultNullableInterface() => DeserializeSlow(DefaultWithNullableIDog, 0);
    [Fact] public void SkipDefaultNullableTuple2() => DeserializeSlow(DefaultWithNullableTuple2, 0);
    [Fact] public void SkipDefaultNullableTuple3() => DeserializeSlow(DefaultWithNullableTuple3, 0);
    [Fact] public void SkipDefaultNullableDictionary() => DeserializeSlow(DefaultWithNullableDictionary, 0);

    [Fact] public void SkipString() => DeserializeSlow(Cat, 0);
    [Fact] public void SkipBool() => DeserializeSlow(WithBool, 0);
    [Fact] public void SkipByte() => DeserializeSlow(WithByte, 0);
    [Fact] public void SkipSByte() => DeserializeSlow(WithSByte, 0);
    [Fact] public void SkipShort() => DeserializeSlow(WithShort, 0);
    [Fact] public void SkipUShort() => DeserializeSlow(WithUShort, 0);
    [Fact] public void SkipInt() => DeserializeSlow(WithInt, 0);
    [Fact] public void SkipUInt() => DeserializeSlow(WithUInt, 0);
    [Fact] public void SkipLong() => DeserializeSlow(WithLong, 0);
    [Fact] public void SkipULong() => DeserializeSlow(WithULong, 0);
    [Fact] public void SkipFloat() => DeserializeSlow(WithFloat, 0);
    [Fact] public void SkipDouble() => DeserializeSlow(WithDouble, 0);
    [Fact] public void SkipDecimal() => DeserializeSlow(WithDecimal, 0);
    [Fact] public void SkipGuid() => DeserializeSlow(WithGuid, 0);

    [Fact] public void SkipNullableString() => DeserializeSlow(WithNullableString, 0);
    [Fact] public void SkipNullableBool() => DeserializeSlow(WithNullableBool, 0);
    [Fact] public void SkipNullableByte() => DeserializeSlow(WithNullableByte, 0);
    [Fact] public void SkipNullableSByte() => DeserializeSlow(WithNullableSByte, 0);
    [Fact] public void SkipNullableShort() => DeserializeSlow(WithNullableShort, 0);
    [Fact] public void SkipNullableUShort() => DeserializeSlow(WithNullableUShort, 0);
    [Fact] public void SkipNullableInt() => DeserializeSlow(WithNullableInt, 0);
    [Fact] public void SkipNullableUInt() => DeserializeSlow(WithNullableUInt, 0);
    [Fact] public void SkipNullableLong() => DeserializeSlow(WithNullableLong, 0);
    [Fact] public void SkipNullableULong() => DeserializeSlow(WithNullableULong, 0);
    [Fact] public void SkipNullableFloat() => DeserializeSlow(WithNullableFloat, 0);
    [Fact] public void SkipNullableDouble() => DeserializeSlow(WithNullableDouble, 0);
    [Fact] public void SkipNullableDecimal() => DeserializeSlow(WithNullableDecimal, 0);
    [Fact] public void SkipNullableGuid() => DeserializeSlow(WithNullableGuid, 0);

    [Fact] public void SkipTurtle1() => DeserializeSlow(Turtle1, 0);
    [Fact] public void SkipTurtle2() => DeserializeSlow(Turtle2, 0);
    [Fact] public void SkipTurtle3() => DeserializeSlow(Turtle3, 0);

    [Fact] public void SkipNode1() => DeserializeSlow(Node1, 0);
    [Fact] public void SkipNode2() => DeserializeSlow(Node2, 0);
    [Fact] public void SkipNode3() => DeserializeSlow(Node3, 0);

    [Fact] public void SkipIAnimal() => DeserializeSlow(IAnimal, 0);
    [Fact] public void SkipIDog() => DeserializeSlow(IDog, 0);

    // Test the code at bookmark 563732229
    [Fact] public void SkipFourInts() => DeserializeSlow(new FourInts(1, 2, 3, 4), DefaultWithInt);
}
