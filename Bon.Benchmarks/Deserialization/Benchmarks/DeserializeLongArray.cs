namespace Bon.Benchmarks.Deserialization.Benchmarks;

public class DeserializeLongArray : ArrayBenchmarkBase<long>
{
    protected override long CreateElement(Random random) => random.NextInt64();
}
