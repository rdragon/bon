namespace Bon.Benchmarks.Deserialization;

partial class BenchmarkBase<TIn, TOut>
{
    [GlobalSetup(Target = nameof(Json))]
    public void SetupJson()
    {
        JsonSerializer.Serialize(_stream, CreateValue());
    }

    [Benchmark]
    public TOut Json()
    {
        _stream.Position = 0;
        return JsonSerializer.Deserialize<TOut>(_stream)!;
    }
}
