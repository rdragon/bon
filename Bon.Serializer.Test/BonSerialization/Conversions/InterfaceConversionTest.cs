namespace Bon.Serializer.Test.BonSerialization.Conversions;

public sealed class InterfaceConversionTest : BonSerializerTestBase
{
    [Fact] public void IDogToIAnimal() => DeserializeSlow(IDog, IAnimal);
    [Fact] public void IDogToICat() => DeserializeSlow(IDog, RealDefaultICat);
    [Fact] public void IDogToDog() => DeserializeSlow(IDog, RealDefaultDog);
    [Fact] public void IDogToInt() => DeserializeSlow(IDog, 0);

    [Fact] public void DogToIDog() => DeserializeSlow(Dog, RealDefaultIDog);
    [Fact] public void IntToIDog() => DeserializeSlow(0, RealDefaultIDog);
}
