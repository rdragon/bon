namespace Bon.Benchmarks.Deserialization.Benchmarks;

public class DeserializeArrayOfSimpleClassV2 : ArrayBenchmarkBase<SimpleClass, SimpleClassV2>
{
    protected override SimpleClass CreateElement(Random random) => new(
        random.Next(),
        new() { Int = random.Next() },
        random.Next(),
        random.Next(),
        random.NextDouble(),
        random.NextDouble() > 0.5);
}

public class DeserializeArrayOfSimpleClassV3 : ArrayBenchmarkBase<SimpleClassV2, SimpleClass>
{
    protected override SimpleClassV2 CreateElement(Random random) => new(
        random.Next(),
        new() { Int = random.Next() },
        random.Next(),
        random.Next(),
        random.NextDouble(),
        random.NextDouble() > 0.5,
        random.Next());
}

[BonObject, ProtoContract(SkipConstructor = true), MessagePackObject]
public sealed record class SimpleClassV2(
     [property: BonMember(1), ProtoMember(1, DataFormat = DataFormat.FixedSize), Key(0)] int Int1,
     [property: BonMember(2), ProtoMember(2, DataFormat = DataFormat.FixedSize), Key(1)] HoldsInt HoldsInt,
     [property: BonMember(3), ProtoMember(3, DataFormat = DataFormat.FixedSize), Key(2)] int Int2,
     [property: BonMember(4), ProtoMember(4, DataFormat = DataFormat.FixedSize), Key(3)] int Int3,
     [property: BonMember(5), ProtoMember(5, DataFormat = DataFormat.FixedSize), Key(4)] double Double,
     [property: BonMember(6), ProtoMember(6, DataFormat = DataFormat.FixedSize), Key(5)] bool Bool,
     [property: BonMember(7), ProtoMember(7, DataFormat = DataFormat.FixedSize), Key(6)] int Int4);

