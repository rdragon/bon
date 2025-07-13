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

    public Action<BinaryWriter, T> GetWriter<T>(SimpleWriterType simpleWriterType) =>
        (Action<BinaryWriter, T>)_writers[(int)simpleWriterType];

    private static void WriteByteNow(BinaryWriter writer, byte value)
    {
        WriteFormatType(writer, FormatType.Byte);
        NativeSerializer.WriteByte(writer, value);
    }

    private static void WriteSByteNow(BinaryWriter writer, sbyte value)
    {
        WriteFormatType(writer, FormatType.SByte);
        NativeSerializer.WriteSByte(writer, value);
    }

    private static void WriteShortNow(BinaryWriter writer, short value)
    {
        WriteFormatType(writer, FormatType.Short);
        NativeSerializer.WriteShort(writer, value);
    }

    private static void WriteUShortNow(BinaryWriter writer, ushort value)
    {
        WriteFormatType(writer, FormatType.UShort);
        NativeSerializer.WriteUShort(writer, value);
    }

    private static void WriteIntNow(BinaryWriter writer, int value)
    {
        WriteFormatType(writer, FormatType.Int);
        NativeSerializer.WriteInt(writer, value);
    }

    private static void WriteUIntNow(BinaryWriter writer, uint value)
    {
        WriteFormatType(writer, FormatType.UInt);
        NativeSerializer.WriteUInt(writer, value);
    }

    private static void WriteLongNow(BinaryWriter writer, long value)
    {
        WriteFormatType(writer, FormatType.Long);
        NativeSerializer.WriteLong(writer, value);
    }

    private static void WriteULongNow(BinaryWriter writer, ulong value)
    {
        WriteFormatType(writer, FormatType.ULong);
        NativeSerializer.WriteULong(writer, value);
    }

    private static void WriteShort(BinaryWriter writer, short value) => WriteLong(writer, value);

    private static void WriteUShort(BinaryWriter writer, ushort value) => WriteULong(writer, value);

    private static void WriteInt(BinaryWriter writer, int value) => WriteLong(writer, value);

    private static void WriteUInt(BinaryWriter writer, uint value) => WriteULong(writer, value);

    private static void WriteLong(BinaryWriter writer, long value)
    {
        if (value >= 0)
        {
            if (value <= byte.MaxValue)
            {
                WriteByteNow(writer, (byte)value);
            }
            else if (value <= short.MaxValue)
            {
                WriteShortNow(writer, (short)value);
            }
            else if (value <= int.MaxValue)
            {
                WriteIntNow(writer, (int)value);
            }
            else
            {
                WriteLongNow(writer, value);
            }
        }
        else if (value >= sbyte.MinValue)
        {
            WriteSByteNow(writer, (sbyte)value);
        }
        else if (value >= short.MinValue)
        {
            WriteShortNow(writer, (short)value);
        }
        else if (value >= int.MinValue)
        {
            WriteIntNow(writer, (int)value);
        }
        else
        {
            WriteLongNow(writer, value);
        }
    }

    private static void WriteULong(BinaryWriter writer, ulong value)
    {
        if (value <= byte.MaxValue)
        {
            WriteByteNow(writer, (byte)value);
        }
        else if (value <= ushort.MaxValue)
        {
            WriteUShortNow(writer, (ushort)value);
        }
        else if (value <= uint.MaxValue)
        {
            WriteUIntNow(writer, (uint)value);
        }
        else
        {
            WriteULongNow(writer, value);
        }
    }

    private static void WriteFormatType(BinaryWriter writer, FormatType formatType) => writer.Write((byte)formatType);
}
