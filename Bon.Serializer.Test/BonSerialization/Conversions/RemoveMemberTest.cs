namespace Bon.Serializer.Test.BonSerialization.Conversions;

public sealed class RemoveMemberTest : BonSerializerTestBase
{
    // Removing members from a class or struct is supported.
    // All members in the serialized data that cannot be found in the target type are skipped.

    // Keep in sync with the file at bookmark 155971713.

    [Fact] public void RemoveFinalIntMemberFromClass() => DeserializeSlow(Dog, EmptyClass);
    [Fact] public void RemoveFinalIntMemberFromStruct() => DeserializeSlow(House, EmptyStruct);
    [Fact] public void RemoveFinalClassMemberFromClass() => DeserializeSlow(WithDog, EmptyClass);
    [Fact] public void RemoveFinalStructMemberFromStruct() => DeserializeSlow(HoldsHouse, EmptyStruct);
    [Fact] public void RemoveFinalInterfaceMemberFromClass() => DeserializeSlow(WithIAnimal, EmptyClass);
    [Fact] public void RemoveFinalNullableClassMemberFromClass() => DeserializeSlow(WithNullableDog, EmptyClass);
    [Fact] public void RemoveFinalNullableStructMemberFromStruct() => DeserializeSlow(HoldsNullableHouse, EmptyStruct);
    [Fact] public void RemoveFinalArrayMemberFromClass() => DeserializeSlow(WithArray, EmptyClass);
    [Fact] public void RemoveFinalStringMemberFromClass() => DeserializeSlow(WithString, EmptyClass);
    [Fact] public void RemoveFinalEnumMemberFromClass() => DeserializeSlow(WithDayOfWeek, EmptyClass);

    [Fact] public void RemoveFirstOfTwoIntMembers() => DeserializeSlow(TwoInts, new WithIntOnPositionTwo(TwoInts.Int2));
    [Fact] public void RemoveSecondOfTwoIntMembers() => DeserializeSlow(TwoInts, new WithInt(TwoInts.Int1));

    [Fact] public void RemoveMiddleOfThreeIntMembers() => DeserializeSlow(ThreeInts, new TwoIntsWithGap(ThreeInts.Int1, ThreeInts.Int3));
    [Fact] public void RemoveLastOfThreeIntMembers() => DeserializeSlow(ThreeInts, CreateTwoInts(ThreeInts.Int1, ThreeInts.Int2));

    [Fact] public void RemoveBothIntMembersFromClass() => DeserializeSlow(TwoInts, EmptyClass);
    [Fact] public void RemoveAllThreeIntMembersFromClass() => DeserializeSlow(ThreeInts, EmptyClass);

    [Fact] public void RemoveTwoIntMembersFromEndOfClass() => DeserializeSlow(ThreeInts, new WithInt(ThreeInts.Int1));
    [Fact] public void RemoveTwoIntMembersFromClass() => DeserializeSlow(ThreeInts, new WithIntOnPositionTwo(ThreeInts.Int2));
}
