namespace Bon.Benchmarks.Deserialization;

public abstract class ArrayBenchmarkBase<T> : BenchmarkBase<T[], T[]>
{
    protected abstract T CreateElement(Random random);

    protected override T[] CreateValue(Random random) => N.CreateValues(() => CreateElement(random));
}

public abstract class ArrayBenchmarkBase<TIn, TOut> : BenchmarkBase<TIn[], TOut[]>
{
    protected abstract TIn CreateElement(Random random);

    protected override TIn[] CreateValue(Random random) => N.CreateValues(() => CreateElement(random));
}
