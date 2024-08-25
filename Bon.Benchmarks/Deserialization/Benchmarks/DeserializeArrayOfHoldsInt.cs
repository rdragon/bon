namespace Bon.Benchmarks.Deserialization.Benchmarks;

public class DeserializeArrayOfHoldsInt : ArrayBenchmarkBase<HoldsInt>
{
    protected override HoldsInt CreateElement(Random random) => new() { Int = random.Next() };
}

public class DeserializeArrayOfHoldsIntV2 : ArrayBenchmarkBase<HoldsIntV2>
{
    protected override HoldsIntV2 CreateElement(Random random) => new() { Int = random.Next() };
}

[BonObject, ProtoContract, MessagePackObject]
public readonly struct HoldsInt
{
    [BonMember(1), ProtoMember(1, DataFormat = DataFormat.FixedSize), Key(0)]
    public required int Int { get; init; }
}

[BonObject, ProtoContract, MessagePackObject]
public readonly struct HoldsIntV2
{
    [BonMember(1), ProtoMember(1), Key("0")]
    public required int Int { get; init; }
}
