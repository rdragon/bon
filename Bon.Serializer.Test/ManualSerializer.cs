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

    public ManualSerializer WriteSchemaType(SchemaType schemaType) => WriteCompactInt((int)schemaType);

    /// <summary>
    /// Writes an int using 1, 2, 3 or 5 bytes.
    /// </summary>
    public ManualSerializer WriteCompactInt(int? value) => WriteWholeNumber((uint?)value);

    public ManualSerializer WriteNull()
    {
        _writer.Write(NativeWriter.NULL);
        return this;
    }

    public ManualSerializer WriteWholeNumber(ulong? value)
    {
        WholeNumberSerializer.Write(_writer, value);
        return this;
    }

    public ManualSerializer WriteSignedWholeNumber(long? value)
    {
        WholeNumberSerializer.WriteSigned(_writer, value);
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

    /// <summary>
    /// Writes an int using 4 bytes.
    /// </summary>
    public ManualSerializer WriteFullInt(int value)
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

    public ManualSerializer WriteClassHeader<T>() =>
        WriteFirstPartOfHeader()
        .WriteClassSchema<T>();

    public ManualSerializer WriteInterfaceHeader<T>() =>
        WriteFirstPartOfHeader()
        .WriteInterfaceSchema<T>();

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

    public ManualSerializer WriteClassSchema<T>() => WriteCustomSchema<T>(GetSchemaTypeOfRecord<T>());

    private static SchemaType GetSchemaTypeOfRecord<T>() => typeof(T).IsNullable() ? SchemaType.NullableRecord : SchemaType.Record;

    public ManualSerializer WriteInterfaceSchema<T>() => WriteCustomSchema<T>(SchemaType.Union);

    private ManualSerializer WriteCustomSchema<T>(SchemaType schemaType) =>
        WriteSchemaType(schemaType)
        .WriteCompactInt(GetContentsId<T>());

    public ManualSerializer WriteNativeHeader(SchemaType schemaType) =>
        WriteFirstPartOfHeader(0)
        .WriteNativeSchema(schemaType);

    public ManualSerializer WriteNativeSchema(SchemaType schemaType) =>
        WriteSchemaType(schemaType);

    public ManualSerializer WriteArraySchemaStart() =>
        WriteSchemaType(SchemaType.Array);

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
        .WriteFullInt(value);

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
