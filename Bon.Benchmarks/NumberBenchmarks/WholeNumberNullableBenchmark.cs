using Bon.Serializer.Serialization;

namespace Bon.Benchmarks.NumberBenchmarks;

public class WholeNumberNullableBenchmark
{
    [Params(100_000)]
    public int N { get; set; }

    [Params(1, 2, 3, 5, 9)]
    public int K { get; set; }

    private MemoryStream _stream = null!;
    private BinaryWriter _writer = null!;
    private BinaryReader _reader = null!;
    private ulong?[] _array = null!;

    public void SanityCheck()
    {
        Console.WriteLine(nameof(WholeNumberNullableBenchmark));
        N = 1000;

        foreach (var k in new[] { 1, 2, 3, 5, 9 })
        {
            K = k;
            SetupDeserialize();
            var sum = Deserialize();
            Console.WriteLine($"K = {k}, size = {_stream.Length}, sum = {sum}");
        }
    }

    private static ulong GetInputValue(int i, int k) => WholeNumberBenchmark.GetInputValue(i, k);

    [GlobalSetup]
    public void Setup()
    {
        _array = new ulong?[N];
        for (int i = 0; i < N; i++)
        {
            _array[i] = GetInputValue(i, K);
        }
        _stream = new MemoryStream();
        _writer = new BinaryWriter(_stream);
        _reader = new BinaryReader(_stream);
    }

    [GlobalSetup(Target = nameof(Deserialize))]
    public void SetupDeserialize()
    {
        Setup();
        Serialize();
    }

    [Benchmark]
    public Stream Serialize()
    {
        _stream.Position = 0;
        for (int i = 0; i < N; i++)
        {
            WholeNumberSerializer.Write(_writer, _array[i]);
        }
        return _stream;
    }

    [Benchmark]
    public ulong Deserialize()
    {
        _stream.Position = 0;
        var sum = 0UL;
        for (int i = 0; i < N; i++)
        {
            sum += WholeNumberSerializer.Read(_reader)!.Value;
        }
        return sum;
    }
}
