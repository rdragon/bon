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
        var writeValue = _writerStore.GetWriter<T>();
        var writer = new BinaryWriter(stream);

        WriteHeader(writer, _blockStore.LastBlockId, typeof(T));
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

        await LoadBlock(blockId).ConfigureAwait(false);
        var schemaData = SchemaSerializer.ReadSchemaData(schemaBytes);
        var schema = _schemaDataResolver.GetSchemaBySchemaData(schemaData);
        var writer = new BinaryWriter(stream);

        WriteHeader(writer, blockId, schemaData);
        JsonToBonSerializer.Serialize(writer, data, schema);
    }

    private void WriteHeader(BinaryWriter writer, uint blockId, Type type)
    {
        var schemaData = _schemaDataStore.GetSchemaData(type);
        WriteHeader(writer, blockId, schemaData);
    }

    private static void WriteHeader(BinaryWriter writer, uint blockId, SchemaData schemaData)
    {
        writer.Write(BON_MARKER);
        writer.Write(VERSION);
        writer.Write(blockId);
        SchemaSerializer.Write(writer, schemaData);
    }
}
