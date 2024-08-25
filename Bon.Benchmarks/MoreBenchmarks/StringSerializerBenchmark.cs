using Bon.Serializer.Serialization;
using System.Text;

namespace Bon.Benchmarks.MoreBenchmarks;

[MemoryDiagnoser]
public class StringSerializerBenchmark
{
    [Params(1_000)]
    public int N { get; set; }

    [Params(1000)]
    public int K { get; set; }

    private MemoryStream _stream = null!;
    private BinaryWriter _writer = null!;
    private BinaryReader _reader = null!;

    private string[] _strings = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _strings = new string[N];

        for (int i = 0; i < N; i++)
        {
            _strings[i] = GetRandomString(random, K);
        }

        _stream = new MemoryStream();
        _writer = new BinaryWriter(_stream);
        _reader = new BinaryReader(_stream);
    }

    [GlobalSetup(Target = nameof(DeserializeFramework))]
    public void SetupDeserializeFramework()
    {
        Setup();
        SerializeFramework();
    }

    [Benchmark]
    public Stream SerializeFramework()
    {
        _stream.Position = 0;

        foreach (var s in _strings)
        {
            _writer.Write(s);
        }

        return _stream;
    }

    [Benchmark]
    public long DeserializeFramework()
    {
        _stream.Position = 0;
        long sum = 0;

        for (int i = 0; i < N; i++)
        {
            sum += _reader.ReadString().Length;
        }

        return sum;
    }

    [GlobalSetup(Target = nameof(DeserializeCustom))]
    public void SetupDeserializeCustom()
    {
        Setup();
        SerializeCustom();
    }

    [Benchmark]
    public Stream SerializeCustom()
    {
        _stream.Position = 0;

        foreach (var s in _strings)
        {
            StringSerializer.WriteString(_writer, s);
        }

        return _stream;
    }

    [Benchmark]
    public long DeserializeCustom()
    {
        _stream.Position = 0;
        long sum = 0;

        for (int i = 0; i < N; i++)
        {
            sum += StringDeserializer.ReadString(_reader)!.Length;
        }

        return sum;
    }

    private static string GetRandomString(Random random, int length)
    {
        var sb = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            sb.Append((char)('A' + random.Next(26)));
        }

        return sb.ToString();
    }
}
