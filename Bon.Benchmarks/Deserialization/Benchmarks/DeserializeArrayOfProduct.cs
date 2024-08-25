namespace Bon.Benchmarks.Deserialization.Benchmarks;

public class DeserializeArrayOfProduct : ArrayBenchmarkBase<Product>
{
    protected override Product CreateElement(Random random) => new(
        random.Next(),
        Enumerable.Range(0, random.Next(10) + 1).Select(_ => random.Next()).ToArray(),
        Enumerable.Range(0, random.Next(10) + 1).Select(_ => new Feature(random.Next(), random.NextSingle())).ToArray());
}

[BonObject, MessagePackObject, ProtoContract(SkipConstructor = true)]
public sealed record class Product(
     [property: BonMember(1), Key(0), ProtoMember(1, DataFormat = DataFormat.FixedSize)] int Int,
     [property: BonMember(2), Key(1), ProtoMember(2, DataFormat = DataFormat.FixedSize)] int[] IntArray,
     [property: BonMember(3), Key(2), ProtoMember(3, DataFormat = DataFormat.FixedSize)] Feature[] Features);

[BonObject, MessagePackObject, ProtoContract(SkipConstructor = true)]
public readonly record struct Feature(
    [property: BonMember(1), Key(0), ProtoMember(1, DataFormat = DataFormat.FixedSize)] int Int,
    [property: BonMember(2), Key(1), ProtoMember(2)] float Float);
