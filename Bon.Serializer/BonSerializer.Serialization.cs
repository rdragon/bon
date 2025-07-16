namespace Bon.Serializer;

partial class BonSerializer
{
    /// <summary>
    /// Serializes a value to a byte array.
    /// </summary>
    public byte[] Serialize<T>(T value, BonSerializerOptions? options = null)
    {
        var stream = new MemoryStream();
        Serialize(stream, value, options);

        return stream.ToArray();
    }

    /// <summary>
    /// Serializes a value to a stream.
    /// </summary>
    public void Serialize<T>(Stream stream, T value, BonSerializerOptions? options = null)
    {
        var writer = new BinaryWriter(stream);

        var (writeValue, usesCustomSchemas, simpleWriterType) = _writerStore.GetWriter<T>();

        if (options?.AllowSchemaTypeOptimization != false && simpleWriterType != SimpleWriterType.None)
        {
            _simpleWriterStore.GetWriter<T>(simpleWriterType)(writer, value);
            return;
        }

        var blockId = usesCustomSchemas ? _blockStore.LastBlockId : 0;

        WriteHeader(writer, blockId, typeof(T));
        writeValue(writer, value);
    }

    /// <param name="blockId">If zero then no block ID is included in the header.</param>
    private void WriteHeader(BinaryWriter writer, uint blockId, Type type)
    {
        var schemaData = _schemaDataStore.GetSchemaData(type);
        WriteHeader(writer, blockId, schemaData);
    }

    /// <param name="blockId">If zero then no block ID is included in the header.</param>
    private static void WriteHeader(BinaryWriter writer, uint blockId, SchemaData schemaData)
    {
        var formatType = GetFormatType(blockId, schemaData);
        writer.Write((byte)formatType);

        if (formatType == FormatType.Full)
        {
            writer.Write(blockId);
        }

        if (formatType is FormatType.Full or FormatType.WithoutBlockId)
        {
            SchemaSerializer.Write(writer, schemaData);
        }
    }

    private static FormatType GetFormatType(uint blockId, SchemaData schemaData)
    {
        return (blockId, schemaData.SchemaType) switch
        {
            (_, SchemaType.Byte) => FormatType.Byte,
            (_, SchemaType.SByte) => FormatType.SByte,
            (_, SchemaType.Short) => FormatType.Short,
            (_, SchemaType.UShort) => FormatType.UShort,
            (_, SchemaType.Int) => FormatType.Int,
            (_, SchemaType.UInt) => FormatType.UInt,
            (_, SchemaType.Long) => FormatType.Long,
            (_, SchemaType.ULong) => FormatType.ULong,
            (0, _) => FormatType.WithoutBlockId,
            _ => FormatType.Full
        };
    }
}
