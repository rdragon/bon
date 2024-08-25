using Bon.Serializer.Serialization;

namespace Bon.Benchmarks.NumberBenchmarks;

public class WholeNumberNullableSignedBenchmark
{
    [Params(100_000)]
    public int N { get; set; }

    [Params(1, 2, 3, 5, 9)]
    public int K { get; set; }

    private MemoryStream _stream = null!;
    private BinaryWriter _writer = null!;
    private BinaryReader _reader = null!;
    private long?[] _array = null!;

    public void SanityCheck()
    {
        Console.WriteLine(nameof(WholeNumberNullableSignedBenchmark));
        N = 1000;

        foreach (var k in new[] { 1, 2, 3, 5, 9 })
        {
            K = k;
            SetupDeserialize();
            var sum = Deserialize();
            Console.WriteLine($"K = {k}, size = {_stream.Length}, sum = {sum}");
        }
    }

    private static long GetInputValue(int i, int k)
    {
        var (min, max) = k switch
        {
            1 => (-60L, 60L),
            2 => (100L, 7000L),
            3 => (20000L, 1000000L),
            5 => (2000000L, 1L << 35),
            9 => (1L << 36, long.MaxValue),
            _ => throw new ArgumentOutOfRangeException(nameof(k))
        };

        return min + (long)((ulong)i * 70663 * 70783 * 42677 % (ulong)(max - min + 1));
    }

    [GlobalSetup]
    public void Setup()
    {
        _array = new long?[N];
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
            WholeNumberSerializer.WriteNullableSigned(_writer, _array[i]);
        }
        return _stream;
    }

    [Benchmark]
    public long Deserialize()
    {
        _stream.Position = 0;
        var sum = 0L;
        for (int i = 0; i < N; i++)
        {
            sum += WholeNumberSerializer.ReadNullableSigned(_reader)!.Value;
        }
        return sum;
    }
}
