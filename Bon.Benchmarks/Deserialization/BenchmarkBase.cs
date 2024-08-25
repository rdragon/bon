namespace Bon.Benchmarks.Deserialization;

[MemoryDiagnoser]
[JsonExporterAttribute.Brief]
public abstract partial class BenchmarkBase<TIn, TOut>
{
    [Params(100_000)]
    public int N { get; set; }

    private readonly MemoryStream _stream = new();

    protected abstract TIn CreateValue(Random random);

    private TIn CreateValue() => CreateValue(new Random(1));

    public long GetStreamLength() => _stream.Length;
}
