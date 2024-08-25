namespace Bon.Benchmarks.Deserialization.Benchmarks;

public class DeserializeArrayOfSimpleClass : ArrayBenchmarkBase<SimpleClass>
{
    protected override SimpleClass CreateElement(Random random) => new(
        random.Next(),
        new() { Int = random.Next() },
        random.Next(),
        random.Next(),
        random.NextDouble(),
        random.NextDouble() > 0.5);
}

[BonObject, ProtoContract(SkipConstructor = true), MessagePackObject]
public sealed record class SimpleClass(
     [property: BonMember(1), ProtoMember(1, DataFormat = DataFormat.FixedSize), Key(0)] int Int1,
     [property: BonMember(2), ProtoMember(2), Key(1)] HoldsInt HoldsInt,
     [property: BonMember(3), ProtoMember(3, DataFormat = DataFormat.FixedSize), Key(2)] int Int2,
     [property: BonMember(4), ProtoMember(4, DataFormat = DataFormat.FixedSize), Key(3)] int Int3,
     [property: BonMember(5), ProtoMember(5), Key(4)] double Double,
     [property: BonMember(6), ProtoMember(6), Key(5)] bool Bool);

