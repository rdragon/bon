namespace Bon.Benchmarks.NumberBenchmarks;

public class Read7BitEncodedIntBenchmark
{
    [Params(10_000)]
    public int N { get; set; }

    [Params(1, 2, 3, 4, 5)]
    public int K { get; set; }

    private MemoryStream _stream = null!;
    private BinaryWriter _writer = null!;
    private BinaryReader _reader = null!;
    private int[] _array = null!;

    public void SanityCheck()
    {
        Console.WriteLine(nameof(Read7BitEncodedIntBenchmark));
        N = 1000;

        foreach (var k in new[] { 1, 2, 3, 4, 5 })
        {
            K = k;
            SetupDeserialize();
            var sum = Deserialize();
            Console.WriteLine($"K = {k}, native size = {_stream.Length}, sum = {sum}");
        }
    }

    private static int GetInputValueLong(int i, int k)
    {
        var min = k == 10 ? int.MinValue : 1L << (k - 1) * 7;
        var max = k == 10 ? 0 : (1L << k * 7) - 1;

        return (int)((uint)min + (uint)i * 70663 * 70783 * 42677 % (uint)(max - min + 1));
    }

    [GlobalSetup]
    public void Setup()
    {
        _array = new int[N];
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
            _writer.Write7BitEncodedInt(_array[i]);
        }
        return _stream;
    }

    [Benchmark]
    public int Deserialize()
    {
        _stream.Position = 0;
        var sum = 0;
        for (int i = 0; i < N; i++)
        {
            sum += _reader.Read7BitEncodedInt();
        }
        return sum;
    }
}
