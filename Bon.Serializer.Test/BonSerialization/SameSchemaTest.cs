namespace Bon.Serializer.Test.BonSerialization;

public sealed class SameSchemaTest : BonSerializerTestBase
{
    [Fact] public void ClassWithSameMembers() => RequireSameSchema<WithString, Cat>();
    [Fact] public void ClassAndStructWithSameMembers() => RequireDifferentSchema<Dog, House>();
    [Fact] public void ClassAndNullableStructWithSameMembers() => RequireSameSchema<Dog, House?>();
    [Fact] public void ClassAndStructWithNoMembers() => RequireDifferentSchema<EmptyClass, EmptyStruct>();
    [Fact] public void ClassAndNullableStructWithNoMembers() => RequireSameSchema<EmptyClass, EmptyStruct?>();
    [Fact] public void InterfaceWithSameMembers() => RequireSameSchema<IAnimal, IAnimalImitation>();

    [Fact] public void ClassWithDifferentMembers() => RequireDifferentSchema<WithString, WithInt>();
    [Fact] public void ClassWithDifferentMemberIds() => RequireDifferentSchema<TwoInts, TwoIntsWithGap>();
    [Fact] public void ClassWithDifferentStringNullability() => RequireSameSchema<WithString, WithNullableString>();
    [Fact] public void ClassWithDifferentClassNullability() => RequireSameSchema<WithDog, WithNullableDog>();
    [Fact] public void StructWithDifferentStructNullability() => RequireDifferentSchema<HoldsHouse, HoldsNullableHouse>();
    [Fact] public void ClassWithDifferentArrayElementNullability() => RequireSameSchema<WithArray, WithArrayOfNullable>();
    [Fact] public void InterfaceWithDifferentMembers() => RequireDifferentSchema<IAnimal, IAnimalFailedImitation>();

    [Fact] public void GenericClass() => RequireSameSchema<Dog, GenericClass<int>>();
    [Fact] public void WithGenericClass() => RequireSameSchema<WithDog, WithGenericClass>();

    private void RequireSameSchema<T1, T2>() => Assert.Equal(PrintSchema<T1>(), PrintSchema<T2>());

    private void RequireDifferentSchema<T1, T2>() => Assert.NotEqual(PrintSchema<T1>(), PrintSchema<T2>());
}
