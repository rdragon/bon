namespace Bon.Serializer.Test.BonSerialization.Conversions;

public sealed class ClassConversionTest : BonSerializerTestBase
{
    [Fact] public void DogToCat() => DeserializeSlow(Dog, Cat);
    [Fact] public void DogToInt() => DeserializeSlow(Dog, 0);
    [Fact] public void CatToDog() => DeserializeSlow(Cat, Dog);
    [Fact] public void IntToDog() => DeserializeSlow(0, DefaultDog);
    [Fact] public void TurtleToTortoise1() => DeserializeSlow(Turtle1, Tortoise1);
    [Fact] public void TurtleToTortoise2() => DeserializeSlow(Turtle2, Tortoise2);
    [Fact] public void TurtleToTortoise3() => DeserializeSlow(Turtle3, Tortoise3);
    [Fact] public void TortoiseToTurtle1() => DeserializeSlow(Tortoise1, Turtle1);
    [Fact] public void TortoiseToTurtle2() => DeserializeSlow(Tortoise2, Turtle2);
    [Fact] public void TortoiseToTurtle3() => DeserializeSlow(Tortoise3, Turtle3);
}
