namespace Bon.Serializer.Test.BonSerialization.Conversions;

public sealed class TupleConversionTest : BonSerializerTestBase
{
    [Fact] public void Tuple2ToCatStringTuple() => DeserializeSlow(Tuple2, AlternativeTuple2);
    [Fact] public void WithTuple2ToWithCatStringTuple() => DeserializeSlow(WithTuple2, WithAlternativeTuple2);

    [Fact] public void Tuple3ToCatStringTuple() => DeserializeSlow(Tuple3, AlternativeTuple3);
    [Fact] public void WithTuple3ToWithCatStringTuple() => DeserializeSlow(WithTuple3, WithAlternativeTuple3);
}
