namespace Bon.Serializer.Serialization;

internal sealed class SimpleWriterStore
{
    private readonly Delegate[] _writers = new Delegate[9];

    public void Initialize()
    {
        _writers[(int)SimpleWriterType.Byte] = WriteByteNow;
        _writers[(int)SimpleWriterType.SByte] = WriteSByteNow;
        _writers[(int)SimpleWriterType.Short] = WriteShort;
        _writers[(int)SimpleWriterType.UShort] = WriteUShort;
        _writers[(int)SimpleWriterType.Int] = WriteInt;
        _writers[(int)SimpleWriterType.UInt] = WriteUInt;
        _writers[(int)SimpleWriterType.Long] = WriteLong;
        _writers[(int)SimpleWriterType.ULong] = WriteULong;
    }

    public Action<BonOutput, T> GetWriter<T>(SimpleWriterType simpleWriterType) =>
        (Action<BonOutput, T>)_writers[(int)simpleWriterType];

    private static void WriteByteNow(BonOutput output, byte value)
    {
        WriteSchemaType(output, SchemaType.Byte);
        NativeSerializer.WriteByte(output.Writer, value);
    }

    private static void WriteSByteNow(BonOutput output, sbyte value)
    {
        WriteSchemaType(output, SchemaType.SByte);
        NativeSerializer.WriteSByte(output.Writer, value);
    }

    private static void WriteShortNow(BonOutput output, short value)
    {
        WriteSchemaType(output, SchemaType.Short);
        NativeSerializer.WriteShort(output.Writer, value);
    }

    private static void WriteUShortNow(BonOutput output, ushort value)
    {
        WriteSchemaType(output, SchemaType.UShort);
        NativeSerializer.WriteUShort(output.Writer, value);
    }

    private static void WriteIntNow(BonOutput output, int value)
    {
        WriteSchemaType(output, SchemaType.Int);
        NativeSerializer.WriteInt(output.Writer, value);
    }

    private static void WriteUIntNow(BonOutput output, uint value)
    {
        WriteSchemaType(output, SchemaType.UInt);
        NativeSerializer.WriteUInt(output.Writer, value);
    }

    private static void WriteLongNow(BonOutput output, long value)
    {
        WriteSchemaType(output, SchemaType.Long);
        NativeSerializer.WriteLong(output.Writer, value);
    }

    private static void WriteULongNow(BonOutput output, ulong value)
    {
        WriteSchemaType(output, SchemaType.ULong);
        NativeSerializer.WriteULong(output.Writer, value);
    }

    private static void WriteShort(BonOutput output, short value) => WriteLong(output, value);

    private static void WriteUShort(BonOutput output, ushort value) => WriteULong(output, value);

    private static void WriteInt(BonOutput output, int value) => WriteLong(output, value);

    private static void WriteUInt(BonOutput output, uint value) => WriteULong(output, value);

    private static void WriteLong(BonOutput output, long value)
    {
        if (value >= 0)
        {
            if (value <= byte.MaxValue)
            {
                WriteByteNow(output, (byte)value);
            }
            else if (value <= short.MaxValue)
            {
                WriteShortNow(output, (short)value);
            }
            else if (value <= int.MaxValue)
            {
                WriteIntNow(output, (int)value);
            }
            else
            {
                WriteLongNow(output, value);
            }
        }
        else if (value >= sbyte.MinValue)
        {
            WriteSByteNow(output, (sbyte)value);
        }
        else if (value >= short.MinValue)
        {
            WriteShortNow(output, (short)value);
        }
        else if (value >= int.MinValue)
        {
            WriteIntNow(output, (int)value);
        }
        else
        {
            WriteLongNow(output, value);
        }
    }

    private static void WriteULong(BonOutput output, ulong value)
    {
        if (value <= byte.MaxValue)
        {
            WriteByteNow(output, (byte)value);
        }
        else if (value <= ushort.MaxValue)
        {
            WriteUShortNow(output, (ushort)value);
        }
        else if (value <= uint.MaxValue)
        {
            WriteUIntNow(output, (uint)value);
        }
        else
        {
            WriteULongNow(output, value);
        }
    }

    private static void WriteSchemaType(BonOutput output, SchemaType schemaType)
    {
        if (output.Options?.IncludeHeader != false)
        {
            output.Writer.Write((byte)schemaType);
        }
    }
}
