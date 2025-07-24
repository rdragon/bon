using System.Text.Json;
using System.Text.Json.Nodes;

namespace Bon.Serializer.Test.BonSerialization;

public sealed class BonToJsonDeserializerTest : BonSerializerTestBase
{
    [Fact] public void TestString() => Run(String);
    [Fact] public void TestBool() => Run(Bool, "1");
    [Fact] public void TestByte() => Run(Byte);
    [Fact] public void TestSByte() => Run(SByte);
    [Fact] public void TestShort() => Run(Short);
    [Fact] public void TestUShort() => Run(UShort);
    [Fact] public void TestInt() => Run(Int);
    [Fact] public void TestUInt() => Run(UInt);
    [Fact] public void TestLong() => Run(Long);
    [Fact] public void TestULong() => Run(ULong);
    [Fact] public void TestFloat() => Run(Float);
    [Fact] public void TestDouble() => Run(Double);
    [Fact] public void TestDecimal() => Run(Decimal);

    [Fact] public void TestNullableString() => Run(NullableString);
    [Fact] public void TestNullableBool() => Run(NullableBool, "1");
    [Fact] public void TestNullableByte() => Run(NullableByte);
    [Fact] public void TestNullableSByte() => Run(NullableSByte);
    [Fact] public void TestNullableShort() => Run(NullableShort);
    [Fact] public void TestNullableUShort() => Run(NullableUShort);
    [Fact] public void TestNullableInt() => Run(NullableInt);
    [Fact] public void TestNullableUInt() => Run(NullableUInt);
    [Fact] public void TestNullableLong() => Run(NullableLong);
    [Fact] public void TestNullableULong() => Run(NullableULong);
    [Fact] public void TestNullableFloat() => Run(NullableFloat, JsonSerializer.Serialize((double?)NullableFloat));
    [Fact] public void TestNullableDouble() => Run(NullableDouble);
    [Fact] public void TestNullableDecimal() => Run(NullableDecimal);

    private static string DogJson => $"[{Age}]";
    private static string OtherDogJson => $"[{OtherAge}]";
    private static string IAnimalJson => $"[{DogId},{DogJson}]";

    [Fact] public void TestDog() => Run(Dog, DogJson);
    [Fact] public void TestWithNullableDog() => Run(WithNullableDog, $"[{DogJson}]");
    [Fact] public void TestDefaultWithNullableDog() => Run(DefaultWithNullableDog, "[null]");
    [Fact] public void TestTwoInts() => Run(TwoInts, $"[{Int1},{Int2}]");

    [Fact] public void TestIAnimal() => Run(IAnimal, IAnimalJson);
    [Fact] public void TestDefaultWithNullableIAnimal() => Run(DefaultWithNullableIAnimal, "[null]");

    [Fact] public void TestTuple2() => Run(Tuple2, $"[{DogJson},{Int}]");
    [Fact] public void TestWithNullableTuple2() => Run(WithNullableTuple2, $"[[{DogJson},{Int}]]");
    [Fact] public void TestDefaultWithNullableTuple2() => Run(DefaultWithNullableTuple2, "[null]");

    [Fact] public void TestTuple3() => Run(Tuple3, $"[{DogJson},{Int},{IAnimalJson}]");
    [Fact] public void TestWithNullableTuple3() => Run(WithNullableTuple3, $"[[{DogJson},{Int},{IAnimalJson}]]");
    [Fact] public void TestDefaultWithNullableTuple3() => Run(DefaultWithNullableTuple3, "[null]");

    [Fact] public void TestArray() => Run(Array, $"[{DogJson},{OtherDogJson}]");
    [Fact] public void TestDefaultWithNullableArray() => Run(DefaultWithNullableArray, "[null]");

    [Fact] public void TestDictionary() => Run(Dictionary, $"[[{Int},{DogJson}]]");

    private void Run<T>(T value)
    {
        var expectedJson = JsonSerializer.Serialize(value);

        Run(value, expectedJson);
    }

    private void Run<T>(T value, string expectedBodyJson)
    {
        var json = DeserializeToJson(value);
        var actualBodyJson = GetBodyJson(json);
        Assert.Equal(expectedBodyJson, actualBodyJson);

        var actualValue = SerializeFromJson<T>(json);
        Assert.Equal(value, actualValue);
    }

    private string DeserializeToJson<T>(T value) => Serialize(value).DeserializeToJson();

    private static string GetBodyJson(string json) => JsonNode.Parse(json)!.AsObject().GetPropertyValue("body").ToJsonString();

    private T? SerializeFromJson<T>(string json)
    {
        var bytes = BonSerializer.JsonToBon(json);

        return BonSerializer.Deserialize<T>(bytes);
    }
}
