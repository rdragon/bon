﻿using System.Text.Json.Nodes;

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

    /// <summary>
    /// The reverse of <see cref="BonToJsonAsync(Stream)"/>.
    /// </summary>
    public async Task JsonToBonAsync(Stream stream, JsonObject jsonObject)
    {
        var blockId = jsonObject.GetPropertyValue("blockId").GetValue<uint>();
        var schemaDataBytes = Convert.FromBase64String(jsonObject.GetPropertyValue("schema").GetValue<string>());
        var body = jsonObject["body"];

        if (blockId != 0)
        {
            await LoadBlock(blockId).ConfigureAwait(false);
        }

        var schemaData = SchemaSerializer.ReadSchemaData(schemaDataBytes);
        var schema = _schemaDataResolver.GetSchemaBySchemaData(schemaData);
        var writer = new BinaryWriter(stream);

        WriteHeader(writer, blockId, schemaData);
        JsonToBonSerializer.Serialize(writer, body, schema);
    }

    /// <summary>
    /// The reverse of <see cref="BonToJsonAsync(string)"/>.
    /// </summary>
    public async Task<byte[]> JsonToBonAsync(string json)
    {
        var stream = new MemoryStream();
        var jsonNode = JsonNode.Parse(json) ?? throw new ArgumentException("Parsing returned null");
        await JsonToBonAsync(stream, jsonNode.AsObject());

        return stream.ToArray();
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
        return (blockId, schemaData.SchemaType, schemaData.IsNullable) switch
        {
            (_, SchemaType.Byte, false) => FormatType.Byte,
            (_, SchemaType.SByte, false) => FormatType.SByte,
            (_, SchemaType.Short, false) => FormatType.Short,
            (_, SchemaType.UShort, false) => FormatType.UShort,
            (_, SchemaType.Int, false) => FormatType.Int,
            (_, SchemaType.UInt, false) => FormatType.UInt,
            (_, SchemaType.Long, false) => FormatType.Long,
            (_, SchemaType.ULong, false) => FormatType.ULong,
            (0, _, _) => FormatType.WithoutBlockId,
            _ => FormatType.Full
        };
    }
}
