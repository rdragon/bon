namespace Bon.Benchmarks.Deserialization.Benchmarks;

public class DeserializeArrayOfPerson : ArrayBenchmarkBase<Person>
{
    protected override Person CreateElement(Random random) => new(
        new string(Enumerable.Range(0, random.Next(20) + 1).Select(_ => (char)random.Next(32, 127)).ToArray()),
        new string(Enumerable.Range(0, random.Next(20) + 1).Select(_ => (char)random.Next(32, 127)).ToArray()),
        new DateTime(random.NextInt64(DateTime.MinValue.Ticks, DateTime.MaxValue.Ticks), DateTimeKind.Utc),
        new DateTime(random.NextInt64(DateTime.MinValue.Ticks, DateTime.MaxValue.Ticks), DateTimeKind.Utc),
        random.Next(),
        random.Next());
}

[BonObject, MessagePackObject, ProtoContract(SkipConstructor = true)]
public sealed record class Person(
     [property: BonMember(1), Key(0), ProtoMember(1)] string String1,
     [property: BonMember(2), Key(1), ProtoMember(2)] string String2,
     [property: BonMember(3), Key(2), ProtoMember(3)] DateTime DateTime1,
     [property: BonMember(4), Key(3), ProtoMember(4)] DateTime DateTime2,
     [property: BonMember(5), Key(4), ProtoMember(5, DataFormat = DataFormat.FixedSize)] int Int1,
     [property: BonMember(6), Key(5), ProtoMember(6, DataFormat = DataFormat.FixedSize)] int Int2);

