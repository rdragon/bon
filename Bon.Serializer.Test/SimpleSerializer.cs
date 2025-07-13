namespace Bon.Serializer.Test;

public sealed class SimpleSerializer : IEnumerable<byte>
{
    private readonly MemoryStream _stream = new();
    private readonly BinaryWriter _writer;
    private readonly BonSerializerTestBase _bonSerializerTestBase;

    public SimpleSerializer(BonSerializerTestBase bonSerializerTestBase)
    {
        _writer = new BinaryWriter(_stream);
        _bonSerializerTestBase = bonSerializerTestBase;
    }

    private int GetContentsId<T>() => _bonSerializerTestBase.GetContentsId<T>();

    public SimpleSerializer WriteWholeNumber(int? value)
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

    public SimpleSerializer WriteInt(int value)
    {
        _writer.Write(value);

        return this;
    }

    public SimpleSerializer WriteUInt(uint value)
    {
        _writer.Write(value);

        return this;
    }

    public SimpleSerializer WriteUShort(ushort value)
    {
        _writer.Write(value);

        return this;
    }

    public SimpleSerializer WriteNullableInt(int? value)
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

    public SimpleSerializer WriteByte(byte value)
    {
        _writer.Write(value);

        return this;
    }

    public SimpleSerializer WriteBool(bool value)
    {
        _writer.Write(value);

        return this;
    }

    public SimpleSerializer WriteClassHeader<T>(bool isNullable = false) =>
        WriteFirstPartOfHeader()
        .WriteClassSchema<T>(isNullable);

    public SimpleSerializer WriteInterfaceHeader<T>(bool isNullable = false) =>
        WriteFirstPartOfHeader()
        .WriteInterfaceSchema<T>(isNullable);

    public SimpleSerializer WriteFirstPartOfHeader(uint? blockId = null)
    {
        var id = blockId ?? _bonSerializerTestBase.BonSerializer.LastBlockId;

        if (id == 0)
        {
            return WriteByte(BonSerializer.NO_BLOCK_ID_FORMAT_TYPE);
        }

        return WriteByte(BonSerializer.DEFAULT_FORMAT_TYPE).WriteUInt(id);
    }

    public SimpleSerializer WriteClassSchema<T>(bool isNullable = false) => WriteCustomSchema<T>(SchemaType.Record, isNullable);

    public SimpleSerializer WriteInterfaceSchema<T>(bool isNullable = false) => WriteCustomSchema<T>(SchemaType.Union, isNullable);

    private SimpleSerializer WriteCustomSchema<T>(SchemaType schemaType, bool isNullable) =>
        WriteWholeNumber((int)schemaType)
        .WriteBool(isNullable)
        .WriteWholeNumber(GetContentsId<T>());

    public SimpleSerializer WriteNativeHeader(SchemaType schemaType, bool isNullable = false) =>
        WriteFirstPartOfHeader()
        .WriteNativeSchema(schemaType, isNullable);

    public SimpleSerializer WriteNativeSchema(SchemaType schemaType, bool isNullable = false) =>
        WriteWholeNumber((int)schemaType)
        .WriteBool(isNullable);

    public SimpleSerializer WriteArraySchemaStart(bool isNullable = false) =>
        WriteWholeNumber((int)SchemaType.Array)
        .WriteBool(isNullable);

    public byte[] ToArray() => _stream.ToArray();

    public IEnumerator<byte> GetEnumerator()
    {
        return ((IEnumerable<byte>)ToArray()).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void ShouldEqual<T>(T value) => Assert.Equal(this, _bonSerializerTestBase.Serialize(value));
}
