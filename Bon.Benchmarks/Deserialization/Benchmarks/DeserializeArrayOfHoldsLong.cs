namespace Bon.Benchmarks.Deserialization.Benchmarks;

public class DeserializeArrayOfHoldsLong : ArrayBenchmarkBase<HoldsLong>
{
    protected override HoldsLong CreateElement(Random random) => new() { Long = random.NextInt64() };
}

[BonObject, ProtoContract, MessagePackObject]
public readonly struct HoldsLong
{
    [BonMember(1), ProtoMember(1, DataFormat = DataFormat.FixedSize), Key(0)]
    public required long Long { get; init; }
}
