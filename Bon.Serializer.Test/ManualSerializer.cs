namespace Bon.Serializer.Test;

public sealed class ManualSerializer : IEnumerable<byte>
{
    private readonly MemoryStream _stream = new();
    private readonly BinaryWriter _writer;
    private readonly BonSerializerTestBase _bonSerializerTestBase;

    public ManualSerializer(BonSerializerTestBase bonSerializerTestBase)
    {
        _writer = new BinaryWriter(_stream);
        _bonSerializerTestBase = bonSerializerTestBase;
    }

    private int GetContentsId<T>() => _bonSerializerTestBase.GetContentsId<T>();

    public ManualSerializer WriteWholeNumber(int value) => WriteWholeNumber((ulong)value);

    public ManualSerializer WriteWholeNumber(ulong? value)
    {
        if (value.HasValue)
        {
            WholeNumberSerializer.Write(_writer, value.Value);
        }
        else
        {
            WholeNumberSerializer.WriteNull(_writer);
        }

        return this;
    }

    public ManualSerializer WriteSignedWholeNumber(long? value)
    {
        if (value.HasValue)
        {
            WholeNumberSerializer.WriteSigned(_writer, value.Value);
        }
        else
        {
            WholeNumberSerializer.WriteNull(_writer);
        }

        return this;
    }

    public ManualSerializer WriteBool(bool value)
    {
        _writer.Write(value);

        return this;
    }

    public ManualSerializer WriteByte(byte value)
    {
        _writer.Write(value);

        return this;
    }

    public ManualSerializer WriteSByte(sbyte value)
    {
        _writer.Write(value);

        return this;
    }

    public ManualSerializer WriteShort(short value)
    {
        _writer.Write(value);

        return this;
    }

    public ManualSerializer WriteUShort(ushort value)
    {
        _writer.Write(value);

        return this;
    }

    public ManualSerializer WriteInt(int value)
    {
        _writer.Write(value);

        return this;
    }

    public ManualSerializer WriteUInt(uint value)
    {
        _writer.Write(value);

        return this;
    }

    public ManualSerializer WriteLong(long value)
    {
        _writer.Write(value);

        return this;
    }

    public ManualSerializer WriteULong(ulong value)
    {
        _writer.Write(value);

        return this;
    }

    public ManualSerializer WriteNullableInt(int? value)
    {
        if (value < TestHelper.NOT_NULL)
        {
            _writer.Write((byte)value);
        }
        else if (value.HasValue)
        {
            _writer.Write(TestHelper.NOT_NULL);
            _writer.Write(value.Value);
        }
        else
        {
            _writer.Write(TestHelper.NULL);
        }

        return this;
    }

    public ManualSerializer WriteClassHeader<T>(bool isNullable = false) =>
        WriteFirstPartOfHeader()
        .WriteClassSchema<T>(isNullable);

    public ManualSerializer WriteInterfaceHeader<T>(bool isNullable = false) =>
        WriteFirstPartOfHeader()
        .WriteInterfaceSchema<T>(isNullable);

    public ManualSerializer WriteFirstPartOfHeader(uint? blockId = null)
    {
        var id = blockId ?? _bonSerializerTestBase.BonSerializer.LastBlockId;

        if (id == 0)
        {
            return WriteFormatType(FormatType.WithoutBlockId);
        }

        return WriteFormatType(FormatType.Full).WriteUInt(id);
    }

    internal ManualSerializer WriteFormatType(FormatType formatType) => WriteByte((byte)formatType);

    public ManualSerializer WriteClassSchema<T>(bool isNullable = false) => WriteCustomSchema<T>(SchemaType.Record, isNullable);

    public ManualSerializer WriteInterfaceSchema<T>(bool isNullable = false) => WriteCustomSchema<T>(SchemaType.Union, isNullable);

    private ManualSerializer WriteCustomSchema<T>(SchemaType schemaType, bool isNullable) =>
        WriteWholeNumber((int)schemaType)
        .WriteBool(isNullable)
        .WriteWholeNumber(GetContentsId<T>());

    public ManualSerializer WriteNativeHeader(SchemaType schemaType, bool isNullable = false) =>
        WriteFirstPartOfHeader(0)
        .WriteNativeSchema(schemaType, isNullable);

    public ManualSerializer WriteNativeSchema(SchemaType schemaType, bool isNullable = false) =>
        WriteWholeNumber((int)schemaType)
        .WriteBool(isNullable);

    public ManualSerializer WriteArraySchemaStart(bool isNullable = false) =>
        WriteWholeNumber((int)SchemaType.Array)
        .WriteBool(isNullable);

    public ManualSerializer WriteByteMessage(byte value) =>
        WriteFormatType(FormatType.Byte)
        .WriteByte(value);

    public ManualSerializer WriteSByteMessage(sbyte value) =>
        WriteFormatType(FormatType.SByte)
        .WriteSByte(value);

    public ManualSerializer WriteShortMessage(short value) =>
        WriteFormatType(FormatType.Short)
        .WriteShort(value);

    public ManualSerializer WriteUShortMessage(ushort value) =>
        WriteFormatType(FormatType.UShort)
        .WriteUShort(value);

    public ManualSerializer WriteIntMessage(int value) =>
        WriteFormatType(FormatType.Int)
        .WriteInt(value);

    public ManualSerializer WriteUIntMessage(uint value) =>
        WriteFormatType(FormatType.UInt)
        .WriteUInt(value);

    public ManualSerializer WriteLongMessage(long value) =>
        WriteFormatType(FormatType.Long)
        .WriteLong(value);

    public ManualSerializer WriteULongMessage(ulong value) =>
        WriteFormatType(FormatType.ULong)
        .WriteULong(value);

    public byte[] ToArray() => _stream.ToArray();

    public IEnumerator<byte> GetEnumerator()
    {
        return ((IEnumerable<byte>)ToArray()).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void ShouldEqual<T>(T value, BonSerializerOptions? options = null) =>
        Assert.Equal(this, _bonSerializerTestBase.Serialize(value, options));
}
