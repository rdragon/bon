namespace Bon.Serializer.Test.BonSerialization;

public sealed class SameSchemaTest : BonSerializerTestBase
{
    [Fact] public void ClassWithSameMembers() => RequireSameSchema<WithString, Cat>();
    [Fact] public void ClassAndStructWithSameMembers() => RequireSameSchema<Dog, House>();
    [Fact] public void ClassAndStructWithNoMembers() => RequireSameSchema<EmptyClass, EmptyStruct>();
    [Fact] public void InterfaceWithSameMembers() => RequireSameSchema<IAnimal, IAnimalImitation>();

    [Fact] public void ClassWithDifferentMembers() => RequireDifferentSchemas<WithString, WithInt>();
    [Fact] public void ClassWithDifferentMemberIds() => RequireDifferentSchemas<TwoInts, TwoIntsWithGap>();
    [Fact] public void ClassWithDifferentStringNullability() => RequireDifferentSchemas<WithString, WithNullableString>();
    [Fact] public void ClassWithDifferentClassNullability() => RequireDifferentSchemas<WithDog, WithNullableDog>();
    [Fact] public void ClassWithDifferentArrayElementNullability() => RequireDifferentSchemas<WithArray, WithArrayOfNullable>();
    [Fact] public void InterfaceWithDifferentMembers() => RequireDifferentSchemas<IAnimal, IAnimalFailedImitation>();

    [Fact] public void GenericClass() => RequireSameSchema<Dog, GenericClass<int>>();
    [Fact] public void WithGenericClass() => RequireSameSchema<WithDog, WithGenericClass>();

    private void RequireSameSchema<T1, T2>() => Assert.Equal(GetContentsId<T1>(), GetContentsId<T2>());

    private void RequireDifferentSchemas<T1, T2>() => Assert.NotEqual(GetContentsId<T1>(), GetContentsId<T2>());
}
