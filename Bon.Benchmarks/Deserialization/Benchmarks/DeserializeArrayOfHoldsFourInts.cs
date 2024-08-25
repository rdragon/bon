namespace Bon.Benchmarks.Deserialization.Benchmarks;

public class DeserializeArrayOfHoldsFourInts : ArrayBenchmarkBase<HoldsFourInts>
{
    protected override HoldsFourInts CreateElement(Random random) => new()
    {
        Int1 = random.Next(),
        Int2 = random.Next(),
        Int3 = random.Next(),
        Int4 = random.Next()
    };
}

[BonObject, ProtoContract, MessagePackObject]
public readonly struct HoldsFourInts
{
    [BonMember(1), ProtoMember(1, DataFormat = DataFormat.FixedSize), Key(0)]
    public required int Int1 { get; init; }

    [BonMember(2), ProtoMember(2, DataFormat = DataFormat.FixedSize), Key(1)]
    public required int Int2 { get; init; }

    [BonMember(3), ProtoMember(3, DataFormat = DataFormat.FixedSize), Key(2)]
    public required int Int3 { get; init; }

    [BonMember(4), ProtoMember(4, DataFormat = DataFormat.FixedSize), Key(3)]
    public required int Int4 { get; init; }
}
