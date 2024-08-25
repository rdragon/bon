namespace Bon.Benchmarks.Deserialization.Benchmarks;

public class DeserializeArrayOfWithInt : ArrayBenchmarkBase<WithInt>
{
    protected override WithInt CreateElement(Random random) => new() { Int = random.Next() };
}

[BonObject, ProtoContract, MessagePackObject]
public sealed class WithInt
{
    [BonMember(1), ProtoMember(1, DataFormat = DataFormat.FixedSize), Key(0)]
    public required int Int { get; init; }
}
