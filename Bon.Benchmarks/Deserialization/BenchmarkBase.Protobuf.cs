namespace Bon.Benchmarks.Deserialization;

partial class BenchmarkBase<TIn, TOut>
{
    [GlobalSetup(Target = nameof(Protobuf))]
    public void SetupProtobuf()
    {
        ProtoBuf.Serializer.Serialize(_stream, CreateValue());
    }

    [Benchmark]
    public TOut Protobuf()
    {
        _stream.Position = 0;
        return ProtoBuf.Serializer.Deserialize<TOut>(_stream);
    }
}
