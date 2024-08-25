namespace Bon.Benchmarks.Deserialization.Benchmarks;

public class DeserializeIntArrayV2 : ArrayBenchmarkBase<bool, int>
{
    protected override bool CreateElement(Random random) => random.NextDouble() > 0;
}

public class DeserializeIntArrayV3 : ArrayBenchmarkBase<short, uint>
{
    protected override short CreateElement(Random random) => (short)random.Next();
}

public class DeserializeIntArrayV4 : ArrayBenchmarkBase<uint, ulong>
{
    protected override uint CreateElement(Random random) => (uint)random.Next();
}
