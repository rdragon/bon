namespace Bon.Benchmarks.Deserialization;

partial class BenchmarkBase<TIn, TOut>
{
    [GlobalSetup(Target = nameof(MessagePack))]
    public void SetupMessagePack()
    {
        MessagePackSerializer.Serialize(_stream, CreateValue());
    }

    [Benchmark]
    public TOut MessagePack()
    {
        _stream.Position = 0;
        return MessagePackSerializer.Deserialize<TOut>(_stream);
    }
}
