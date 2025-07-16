namespace Bon.Serializer.Test.BonSerialization.Conversions;

public sealed class InterfaceMemberConversionTest : BonSerializerTestBase
{
    [Fact] public void WithIDogToWithNullableIDog() => DeserializeFast(WithIDog, WithNullableIDog);
    [Fact] public void WithIDogToWithIAnimal() => DeserializeSlow(WithIDog, WithIAnimal);
    [Fact] public void WithIDogToWithNullableIAnimal() => DeserializeSlow(WithIDog, WithNullableIAnimal);
    [Fact] public void WithIDogToWithICat() => DeserializeSlow(WithIDog, DefaultWithNullICat);
    [Fact] public void WithIDogToWithNullableICat() => DeserializeSlow(WithIDog, DefaultWithNullableICat);
    [Fact] public void WithIDogToWithDog() => DeserializeSlow(WithIDog, DefaultWithDogNull);
    [Fact] public void WithIDogToWithNullableDog() => DeserializeSlow(WithIDog, DefaultWithNullableDog);
    [Fact] public void WithIDogToDog() => DeserializeSlow(WithIDog, DefaultDog);
    [Fact] public void WithIDogToIAnimal() => DeserializeSlow(WithIDog, RealDefaultIAnimal);
    [Fact] public void WithIDogToInt() => DeserializeSlow(WithIDog, 0);

    [Fact] public void WithNullableIDogToWithIDog() => DeserializeFast(WithNullableIDog, WithIDog);
    [Fact] public void WithNullableIDogToWithIAnimal() => DeserializeSlow(WithNullableIDog, WithIAnimal);
    [Fact] public void WithNullableIDogToWithNullableIAnimal() => DeserializeSlow(WithNullableIDog, WithNullableIAnimal);
    [Fact] public void WithNullableIDogToWithICat() => DeserializeSlow(WithNullableIDog, DefaultWithNullICat);
    [Fact] public void WithNullableIDogToWithNullableICat() => DeserializeSlow(WithNullableIDog, DefaultWithNullableICat);
    [Fact] public void WithNullableIDogToWithDog() => DeserializeSlow(WithNullableIDog, DefaultWithDogNull);
    [Fact] public void WithNullableIDogToWithNullableDog() => DeserializeSlow(WithNullableIDog, DefaultWithNullableDog);
    [Fact] public void WithNullableIDogToDog() => DeserializeSlow(WithNullableIDog, DefaultDog);
    [Fact] public void WithNullableIDogToIAnimal() => DeserializeSlow(WithNullableIDog, RealDefaultIAnimal);
    [Fact] public void WithNullableIDogToInt() => DeserializeSlow(WithNullableIDog, 0);

    [Fact] public void DefaultWithNullableIDogToWithIDog() => DeserializeFast(DefaultWithNullableIDog, RealDefaultWithIDog);
    [Fact] public void DefaultWithNullableIDogToWithIAnimal() => DeserializeSlow(DefaultWithNullableIDog, DefaultWithNullIAnimal);
    [Fact] public void DefaultWithNullableIDogToWithNullableIAnimal() => DeserializeSlow(DefaultWithNullableIDog, DefaultWithNullableIAnimal);
    [Fact] public void DefaultWithNullableIDogToWithICat() => DeserializeSlow(DefaultWithNullableIDog, DefaultWithNullICat);
    [Fact] public void DefaultWithNullableIDogToWithNullableICat() => DeserializeSlow(DefaultWithNullableIDog, DefaultWithNullableICat);
    [Fact] public void DefaultWithNullableIDogToWithDog() => DeserializeSlow(DefaultWithNullableIDog, DefaultWithDogNull);
    [Fact] public void DefaultWithNullableIDogToWithNullableDog() => DeserializeSlow(DefaultWithNullableIDog, DefaultWithNullableDog);
    [Fact] public void DefaultWithNullableIDogToDog() => DeserializeSlow(DefaultWithNullableIDog, DefaultDog);
    [Fact] public void DefaultWithNullableIDogToIAnimal() => DeserializeSlow(DefaultWithNullableIDog, RealDefaultIAnimal);
    [Fact] public void DefaultWithNullableIDogToInt() => DeserializeSlow(DefaultWithNullableIDog, 0);
}
