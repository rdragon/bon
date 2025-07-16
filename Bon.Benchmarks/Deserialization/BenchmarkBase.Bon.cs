namespace Bon.Benchmarks.Deserialization;

partial class BenchmarkBase<TIn, TOut>
{
    private BonSerializer _bonSerializer = null!;

    [GlobalSetup(Target = nameof(Bon))]
    public async Task SetupBon()
    {
        _bonSerializer = await BonSerializer.CreateAsync(new BonSerializerContext(), new InMemoryBlob());
        _bonSerializer.Serialize(_stream, CreateValue());
    }

    [Benchmark(Baseline = true)]
    public TOut Bon()
    {
        _stream.Position = 0;
        return _bonSerializer.Deserialize<TOut>(_stream)!;
    }
}
