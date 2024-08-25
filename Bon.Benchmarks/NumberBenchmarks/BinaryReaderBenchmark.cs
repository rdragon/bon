namespace Bon.Benchmarks.NumberBenchmarks;

public class BinaryReaderBenchmark
{
    [Params(10_000)]
    public int N { get; set; }

    private MemoryStream _stream = null!;
    private BinaryWriter _writer = null!;
    private BinaryReader _reader = null!;

    private bool[] _bools = null!;
    private byte[] _bytes = null!;
    private sbyte[] _sbytes = null!;
    private short[] _shorts = null!;
    private ushort[] _ushorts = null!;
    private int[] _ints = null!;
    private uint[] _uints = null!;
    private long[] _longs = null!;
    private ulong[] _ulongs = null!;
    private float[] _floats = null!;
    private double[] _doubles = null!;

    [GlobalSetup]
    public void Setup()
    {
        _bools = new bool[N];
        _bytes = new byte[N];
        _sbytes = new sbyte[N];
        _shorts = new short[N];
        _ushorts = new ushort[N];
        _ints = new int[N];
        _uints = new uint[N];
        _longs = new long[N];
        _ulongs = new ulong[N];
        _floats = new float[N];
        _doubles = new double[N];

        for (int i = 0; i < N; i++)
        {
            _bools[i] = i % 2 == 0;
            _bytes[i] = (byte)i;
            _sbytes[i] = (sbyte)i;
            _shorts[i] = (short)i;
            _ushorts[i] = (ushort)i;
            _ints[i] = i;
            _uints[i] = (uint)i;
            _longs[i] = i;
            _ulongs[i] = (ulong)i;
            _floats[i] = i;
            _doubles[i] = i;
        }

        _stream = new MemoryStream();
        _writer = new BinaryWriter(_stream);
        _reader = new BinaryReader(_stream);
    }

    [GlobalSetup(Target = nameof(DeserializeBools))]
    public void SetupDeserializeBools()
    {
        Setup();
        SerializeBools();
    }

    [GlobalSetup(Target = nameof(DeserializeBytes))]
    public void SetupDeserializeBytes()
    {
        Setup();
        SerializeBytes();
    }

    [GlobalSetup(Target = nameof(DeserializeSBytes))]
    public void SetupDeserializeSBytes()
    {
        Setup();
        SerializeSBytes();
    }

    [GlobalSetup(Target = nameof(DeserializeShorts))]
    public void SetupDeserializeShorts()
    {
        Setup();
        SerializeShorts();
    }

    [GlobalSetup(Target = nameof(DeserializeUShorts))]
    public void SetupDeserializeUShorts()
    {
        Setup();
        SerializeUShorts();
    }

    [GlobalSetup(Target = nameof(DeserializeInts))]
    public void SetupDeserializeInts()
    {
        Setup();
        SerializeInts();
    }

    [GlobalSetup(Target = nameof(DeserializeUInts))]
    public void SetupDeserializeUInts()
    {
        Setup();
        SerializeUInts();
    }

    [GlobalSetup(Target = nameof(DeserializeLongs))]
    public void SetupDeserializeLongs()
    {
        Setup();
        SerializeLongs();
    }

    [GlobalSetup(Target = nameof(DeserializeULongs))]
    public void SetupDeserializeULongs()
    {
        Setup();
        SerializeULongs();
    }

    [GlobalSetup(Target = nameof(DeserializeFloats))]
    public void SetupDeserializeFloats()
    {
        Setup();
        SerializeFloats();
    }

    [GlobalSetup(Target = nameof(DeserializeDoubles))]
    public void SetupDeserializeDoubles()
    {
        Setup();
        SerializeDoubles();
    }

    [Benchmark]
    public void SerializeBools()
    {
        _stream.Position = 0;
        for (int i = 0; i < N; i++)
        {
            _writer.Write(_bools[i]);
        }
    }

    [Benchmark]
    public void SerializeBytes()
    {
        _stream.Position = 0;
        for (int i = 0; i < N; i++)
        {
            _writer.Write(_bytes[i]);
        }
    }

    [Benchmark]
    public void SerializeByteArray()
    {
        _stream.Position = 0;
        _writer.Write(_bytes);
    }

    [Benchmark]
    public void SerializeSBytes()
    {
        _stream.Position = 0;
        for (int i = 0; i < N; i++)
        {
            _writer.Write(_sbytes[i]);
        }
    }

    [Benchmark]
    public void SerializeShorts()
    {
        _stream.Position = 0;
        for (int i = 0; i < N; i++)
        {
            _writer.Write(_shorts[i]);
        }
    }

    [Benchmark]
    public void SerializeUShorts()
    {
        _stream.Position = 0;
        for (int i = 0; i < N; i++)
        {
            _writer.Write(_ushorts[i]);
        }
    }

    [Benchmark]
    public void SerializeInts()
    {
        _stream.Position = 0;
        for (int i = 0; i < N; i++)
        {
            _writer.Write(_ints[i]);
        }
    }

    [Benchmark]
    public void SerializeUInts()
    {
        _stream.Position = 0;
        for (int i = 0; i < N; i++)
        {
            _writer.Write(_uints[i]);
        }
    }

    [Benchmark]
    public void SerializeLongs()
    {
        _stream.Position = 0;
        for (int i = 0; i < N; i++)
        {
            _writer.Write(_longs[i]);
        }
    }

    [Benchmark]
    public void SerializeULongs()
    {
        _stream.Position = 0;
        for (int i = 0; i < N; i++)
        {
            _writer.Write(_ulongs[i]);
        }
    }

    [Benchmark]
    public void SerializeFloats()
    {
        _stream.Position = 0;
        for (int i = 0; i < N; i++)
        {
            _writer.Write(_floats[i]);
        }
    }

    [Benchmark]
    public void SerializeDoubles()
    {
        _stream.Position = 0;
        for (int i = 0; i < N; i++)
        {
            _writer.Write(_doubles[i]);
        }
    }

    [Benchmark]
    public bool DeserializeBools()
    {
        var result = false;
        _stream.Position = 0;
        for (int i = 0; i < N; i++)
        {
            result ^= _reader.ReadBoolean();
        }
        return result;
    }

    [Benchmark]
    public byte DeserializeBytes()
    {
        byte result = 0;
        _stream.Position = 0;
        for (int i = 0; i < N; i++)
        {
            result += _reader.ReadByte();
        }
        return result;
    }

    [Benchmark]
    public sbyte DeserializeSBytes()
    {
        sbyte result = 0;
        _stream.Position = 0;
        for (int i = 0; i < N; i++)
        {
            result += _reader.ReadSByte();
        }
        return result;
    }

    [Benchmark]
    public short DeserializeShorts()
    {
        short result = 0;
        _stream.Position = 0;
        for (int i = 0; i < N; i++)
        {
            result += _reader.ReadInt16();
        }
        return result;
    }

    [Benchmark]
    public ushort DeserializeUShorts()
    {
        ushort result = 0;
        _stream.Position = 0;
        for (int i = 0; i < N; i++)
        {
            result += _reader.ReadUInt16();
        }
        return result;
    }

    [Benchmark]
    public int DeserializeInts()
    {
        int result = 0;
        _stream.Position = 0;
        for (int i = 0; i < N; i++)
        {
            result += _reader.ReadInt32();
        }
        return result;
    }

    [Benchmark]
    public uint DeserializeUInts()
    {
        uint result = 0;
        _stream.Position = 0;
        for (int i = 0; i < N; i++)
        {
            result += _reader.ReadUInt32();
        }
        return result;
    }

    [Benchmark]
    public long DeserializeLongs()
    {
        long result = 0;
        _stream.Position = 0;
        for (int i = 0; i < N; i++)
        {
            result += _reader.ReadInt64();
        }
        return result;
    }

    [Benchmark]
    public ulong DeserializeULongs()
    {
        ulong result = 0;
        _stream.Position = 0;
        for (int i = 0; i < N; i++)
        {
            result += _reader.ReadUInt64();
        }
        return result;
    }

    [Benchmark]
    public float DeserializeFloats()
    {
        float result = 0;
        _stream.Position = 0;
        for (int i = 0; i < N; i++)
        {
            result += _reader.ReadSingle();
        }
        return result;
    }

    [Benchmark]
    public double DeserializeDoubles()
    {
        double result = 0;
        _stream.Position = 0;
        for (int i = 0; i < N; i++)
        {
            result += _reader.ReadDouble();
        }
        return result;
    }
}
