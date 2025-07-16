
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

    private int GetLayoutId<T>() => _bonSerializerTestBase.GetLayoutId<T>();

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

    public ManualSerializer WriteNotNull()
    {
        _writer.Write(NativeWriter.NOT_NULL);
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

    public ManualSerializer WriteClassSchema<T>(bool isNullable = true) => WriteCustomSchema<T>(isNullable ? SchemaType.NullableRecord : SchemaType.Record);

    public ManualSerializer WriteInterfaceSchema<T>() => WriteCustomSchema<T>(SchemaType.Union);

    private ManualSerializer WriteCustomSchema<T>(SchemaType schemaType) =>
        WriteSchemaType(schemaType)
        .WriteCompactInt(GetLayoutId<T>());

    public ManualSerializer WriteNativeHeader(SchemaType schemaType) =>
        WriteNativeSchema(schemaType);

    public ManualSerializer WriteNativeSchema(SchemaType schemaType) =>
        WriteSchemaType(schemaType);

    public ManualSerializer WriteArraySchemaStart() =>
        WriteSchemaType(SchemaType.Array);

    public ManualSerializer WriteByteMessage(byte value) =>
        WriteSchemaType(SchemaType.Byte)
        .WriteByte(value);

    public ManualSerializer WriteSByteMessage(sbyte value) =>
        WriteSchemaType(SchemaType.SByte)
        .WriteSByte(value);

    public ManualSerializer WriteShortMessage(short value) =>
        WriteSchemaType(SchemaType.Short)
        .WriteShort(value);

    public ManualSerializer WriteUShortMessage(ushort value) =>
        WriteSchemaType(SchemaType.UShort)
        .WriteUShort(value);

    public ManualSerializer WriteIntMessage(int value) =>
        WriteSchemaType(SchemaType.Int)
        .WriteFullInt(value);

    public ManualSerializer WriteUIntMessage(uint value) =>
        WriteSchemaType(SchemaType.UInt)
        .WriteUInt(value);

    public ManualSerializer WriteLongMessage(long value) =>
        WriteSchemaType(SchemaType.Long)
        .WriteLong(value);

    public ManualSerializer WriteULongMessage(ulong value) =>
        WriteSchemaType(SchemaType.ULong)
        .WriteULong(value);

    public byte[] ToArray() => _stream.ToArray();

    public IEnumerator<byte> GetEnumerator()
    {
        return ((IEnumerable<byte>)ToArray()).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void ShouldBeMessageFor<T>(T value, BonSerializerOptions? options = null) =>
        Assert.Equal(this, _bonSerializerTestBase.Serialize(value, options));

    public void ShouldBeBodyFor<T>(T value, BonSerializerOptions? options = null) =>
        Assert.Equal(this, _bonSerializerTestBase.Serialize(value, RemoveHeader(options)));

    private static BonSerializerOptions RemoveHeader(BonSerializerOptions? options)
    {
        if (options is null)
        {
            return new BonSerializerOptions { IncludeHeader = false };
        }
        else
        {
            options.IncludeHeader = false;
            return options;
        }
    }
}
