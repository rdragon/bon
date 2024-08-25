namespace Bon.Benchmarks.Deserialization.Benchmarks;

public class DeserializeArrayOfWithTwoInts : ArrayBenchmarkBase<WithTwoInts>
{
    protected override WithTwoInts CreateElement(Random random) => new() { Int1 = random.Next(), Int2 = random.Next() };
}

[BonObject, ProtoContract, MessagePackObject]
public sealed class WithTwoInts
{
    [BonMember(1), ProtoMember(1, DataFormat = DataFormat.FixedSize), Key(0)]
    public required int Int1 { get; init; }

    [BonMember(2), ProtoMember(2, DataFormat = DataFormat.FixedSize), Key(1)]
    public required int Int2 { get; init; }
}
