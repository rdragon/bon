namespace Bon.Benchmarks.Deserialization.Benchmarks;

public class DeserializeArrayOfHoldsTwoInts : ArrayBenchmarkBase<HoldsTwoInts>
{
    protected override HoldsTwoInts CreateElement(Random random) => new() { Int1 = random.Next(), Int2 = random.Next() };
}

[BonObject, ProtoContract, MessagePackObject]
public readonly struct HoldsTwoInts
{
    [BonMember(1), ProtoMember(1, DataFormat = DataFormat.FixedSize), Key(0)]
    public required int Int1 { get; init; }

    [BonMember(2), ProtoMember(2, DataFormat = DataFormat.FixedSize), Key(1)]
    public required int Int2 { get; init; }
}
