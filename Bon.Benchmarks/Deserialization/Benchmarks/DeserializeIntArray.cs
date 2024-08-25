namespace Bon.Benchmarks.Deserialization.Benchmarks;

public class DeserializeIntArray : ArrayBenchmarkBase<int>
{
    protected override int CreateElement(Random random) => random.Next();
}
