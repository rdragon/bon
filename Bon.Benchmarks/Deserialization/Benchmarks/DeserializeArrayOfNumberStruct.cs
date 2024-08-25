namespace Bon.Benchmarks.Deserialization.Benchmarks;

public class DeserializeArrayOfNumberStruct : ArrayBenchmarkBase<NumberStruct>
{
    protected override NumberStruct CreateElement(Random random) => new(
        random.NextInt64(),
        random.Next(),
        (short)random.Next(),
        (byte)random.Next(),
        random.NextDouble() > 0.5);
}

[BonObject, MessagePackObject, ProtoContract(SkipConstructor = true)]
public readonly record struct NumberStruct(
     [property: BonMember(1), Key(0), ProtoMember(1, DataFormat = DataFormat.FixedSize)] long Long,
     [property: BonMember(2), Key(1), ProtoMember(2, DataFormat = DataFormat.FixedSize)] int Int,
     [property: BonMember(3), Key(2), ProtoMember(3, DataFormat = DataFormat.FixedSize)] short Short,
     [property: BonMember(4), Key(3), ProtoMember(4, DataFormat = DataFormat.FixedSize)] byte Byte,
     [property: BonMember(5), Key(4), ProtoMember(5, DataFormat = DataFormat.FixedSize)] bool Bool);

