namespace Bon.Benchmarks.NumberBenchmarks;

public class Read7BitEncodedInt64Benchmark
{
    [Params(10_000)]
    public int N { get; set; }

    [Params(1, 2, 3, 4, 5, 6, 7, 8, 9, 10)]
    public int K { get; set; }

    private MemoryStream _stream = null!;
    private BinaryWriter _writer = null!;
    private BinaryReader _reader = null!;
    private long[] _array = null!;

    public void SanityCheck()
    {
        Console.WriteLine(nameof(Read7BitEncodedInt64Benchmark));
        N = 1000;

        foreach (var k in new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 })
        {
            K = k;
            SetupDeserialize();
            var sum = Deserialize();
            Console.WriteLine($"K = {k}, native size = {_stream.Length}, sum = {sum}");
        }
    }

    private static long GetInputValueLong(int i, int k)
    {
        var j = (ulong)i * 70663 * 70783 * 42677;

        if (k == 10)
        {
            return -(long)(j % long.MaxValue) - 1;
        }

        var min = 1L << (k - 1) * 7;
        var max = (1L << k * 7) - 1;

        return min + (long)(j % (ulong)(max - min + 1));
    }

    [GlobalSetup]
    public void Setup()
    {
        _array = new long[N];
        for (int i = 0; i < N; i++)
        {
            _array[i] = GetInputValueLong(i, K);
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
            _writer.Write7BitEncodedInt64(_array[i]);
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
            sum += _reader.Read7BitEncodedInt64();
        }
        return sum;
    }
}
