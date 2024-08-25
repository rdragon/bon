namespace Bon.Serializer.Test.BonSerialization.Conversions;

public sealed class AddMemberTest : BonSerializerTestBase
{
    // Adding members to a class or struct is supported.
    // The new member receives the default value if it is not present in the serialized data.

    // Keep in sync with the file at bookmark 155971713.

    [Fact] public void AddIntMemberToEmptyClass() => DeserializeSlow(EmptyClass, DefaultDog);
    [Fact] public void AddIntMemberToEmptyStruct() => DeserializeSlow(EmptyStruct, DefaultHouse);
    [Fact] public void AddClassMemberToEmptyClass() => DeserializeSlow(EmptyClass, DefaultWithDog);
    [Fact] public void AddStructMemberToEmptyStruct() => DeserializeSlow(EmptyStruct, DefaultHoldsHouse);
    [Fact] public void AddInterfaceMemberToEmptyClass() => DeserializeSlow(EmptyClass, DefaultWithIAnimal);
    [Fact] public void AddNullableClassMemberToEmptyClass() => DeserializeSlow(EmptyClass, DefaultWithNullableDog);
    [Fact] public void AddNullableStructMemberToEmptyStruct() => DeserializeSlow(EmptyStruct, DefaultHoldsNullableHouse);
    [Fact] public void AddArrayMemberToEmptyClass() => DeserializeSlow(EmptyClass, DefaultWithArray);
    [Fact] public void AddStringMemberToEmptyClass() => DeserializeSlow(EmptyClass, DefaultWithString);
    [Fact] public void AddEnumMemberToEmptyClass() => DeserializeSlow(EmptyClass, DefaultWithDayOfWeek);

    [Fact] public void AddSecondIntMemberToStartOfClass() => DeserializeSlow(WithIntOnPositionTwo, CreateTwoInts(0, Int));
    [Fact] public void AddSecondIntMemberToEndOfClass() => DeserializeSlow(Dog, CreateTwoInts(Dog.Age, 0));

    [Fact] public void AddThirdIntMemberToMiddleOfClass() => DeserializeSlow(TwoIntsWithGap, new ThreeInts(TwoIntsWithGap.Int1, 0, TwoIntsWithGap.Int2));
    [Fact] public void AddThirdIntMemberToEndOfClass() => DeserializeSlow(TwoInts, new ThreeInts(TwoInts.Int1, TwoInts.Int2, 0));

    [Fact] public void AddTwoIntMembersToEmptyClass() => DeserializeSlow(EmptyClass, DefaultTwoInts);
    [Fact] public void AddThreeIntMembersToEmptyClass() => DeserializeSlow(EmptyClass, DefaultThreeInts);

    [Fact] public void AddTwoIntMembersToEndClass() => DeserializeSlow(Dog, new ThreeInts(Dog.Age, 0, 0));
    [Fact] public void AddTwoIntMembersToClass() => DeserializeSlow(WithIntOnPositionTwo, new ThreeInts(0, WithIntOnPositionTwo.Int, 0));
}
