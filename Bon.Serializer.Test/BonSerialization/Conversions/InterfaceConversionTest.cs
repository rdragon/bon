﻿namespace Bon.Serializer.Test.BonSerialization.Conversions;

public sealed class InterfaceConversionTest : BonSerializerTestBase
{
    [Fact] public void IDogToIAnimal() => DeserializeSlow(IDog, IAnimal);
    [Fact] public void IDogToICat() => DeserializeSlow(IDog, DefaultICat);
    [Fact] public void IDogToDog() => DeserializeSlow(IDog, DefaultDog);
    [Fact] public void IDogToInt() => DeserializeSlow(IDog, 0);

    [Fact] public void DogToIDog() => DeserializeSlow(Dog, DefaultIDog);
    [Fact] public void IntToIDog() => DeserializeSlow(0, DefaultIDog);
}
