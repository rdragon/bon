using System.Text.Json.Nodes;

namespace Bon.Serializer;

partial class BonSerializer
{
    /// <summary>
    /// Serializes a value to a byte array.
    /// </summary>
    public byte[] Serialize<T>(T value)
    {
        var stream = new MemoryStream();
        Serialize(stream, value);

        return stream.ToArray();
    }

    /// <summary>
    /// Serializes a value to a stream.
    /// </summary>
    public void Serialize<T>(Stream stream, T value)
    {
        var writer = new BinaryWriter(stream);

        var (writeValue, usesCustomSchemas) = _writerStore.GetWriter<T>();
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
        var schemaBytes = Convert.FromBase64String(jsonObject.GetPropertyValue("schema").GetValue<string>());
        var data = jsonObject["data"];

        if (blockId != 0)
        {
            await LoadBlock(blockId).ConfigureAwait(false);
        }

        var schemaData = SchemaSerializer.ReadSchemaData(schemaBytes);
        var schema = _schemaDataResolver.GetSchemaBySchemaData(schemaData);
        var writer = new BinaryWriter(stream);

        WriteHeader(writer, blockId, schemaData);
        JsonToBonSerializer.Serialize(writer, data, schema);
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
        if (blockId != 0)
        {
            writer.Write(DEFAULT_FORMAT_TYPE);
            writer.Write(blockId);
        }
        else
        {
            writer.Write(NO_BLOCK_ID_FORMAT_TYPE);
        }

        SchemaSerializer.Write(writer, schemaData);
    }
}
