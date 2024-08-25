using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;

namespace Bon.Serializer;

partial class BonSerializer
{
    /// <summary>
    /// Deserializes a value from a byte array.
    /// Loads new schemas from the storage if unknown schema IDs are encountered.
    /// </summary>
    public Task<T> DeserializeAsync<T>(byte[] bytes) => DeserializeAsync<T>(new MemoryStream(bytes));

    /// <summary>
    /// Deserializes a value from a stream.
    /// Loads new schemas from the storage if unknown schema IDs are encountered.
    /// </summary>
    public async Task<T> DeserializeAsync<T>(Stream stream)
    {
        var reader = new BinaryReader(stream);
        var (_, schemaData) = await ReadHeader(reader).ConfigureAwait(false);

        return DeserializeBody<T>(reader, schemaData);
    }

    /// <summary>
    /// Deserializes a value from a stream.
    /// If unknown schema IDs are encountered then false is returned and the out parameter is set to the default value.
    /// </summary>
    public bool TryDeserialize<T>(Stream stream, [MaybeNullWhen(false)] out T value)
    {
        var reader = new BinaryReader(stream);

        if (TryReadHeader(reader) is SchemaData schemaData)
        {
            value = DeserializeBody<T>(reader, schemaData);
            return true;
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Deserializes a value from a stream and then converts the value to a <see cref="JsonObject"/>.
    /// Loads new schemas from the storage if unknown schema IDs are encountered.
    /// </summary>
    public async Task<JsonObject> BonToJsonAsync(Stream stream)
    {
        var reader = new BinaryReader(stream);
        var (blockId, schemaData) = await ReadHeader(reader).ConfigureAwait(false);
        var schemaBase64 = Convert.ToBase64String(SchemaSerializer.Write(schemaData));
        var schema = _schemaDataResolver.GetSchemaBySchemaData(schemaData);

        return new JsonObject
        {
            ["blockId"] = blockId,
            ["schema"] = schemaBase64,
            ["data"] = BonToJsonDeserializer.Deserialize(reader, schema),
        };
    }

    private async Task<(uint BlockId, SchemaData SchemaData)> ReadHeader(BinaryReader reader)
    {
        var blockId = ReadFirstPartOfHeader(reader);
        await LoadBlock(blockId).ConfigureAwait(false);
        var schemaData = ReadSecondPartOfHeader(reader);

        return (blockId, schemaData);
    }

    private SchemaData? TryReadHeader(BinaryReader reader)
    {
        var blockId = ReadFirstPartOfHeader(reader);

        return BlockIsLoaded(blockId) ? ReadSecondPartOfHeader(reader) : null;
    }

    private static uint ReadFirstPartOfHeader(BinaryReader reader)
    {
        var marker = reader.ReadUInt16();

        if (marker != BON_MARKER)
        {
            throw new DeserializationFailedException($"Invalid stream. Expected marker {BON_MARKER:x4} but found {marker:x4}.");
        }

        var version = reader.ReadByte();

        if (version > 0)
        {
            throw new DeserializationFailedException($"Invalid stream. Cannot handle wire format version {version}.");
        }

        return reader.ReadUInt32();
    }

    private static SchemaData ReadSecondPartOfHeader(BinaryReader reader) => SchemaSerializer.ReadSchemaData(reader);

    private T DeserializeBody<T>(BinaryReader reader, SchemaData schemaData)
    {
        var schema = _schemaDataResolver.GetSchemaBySchemaData(schemaData);
        var deserialize = _deserializerStore.GetDeserializer<T>(schema, null);
        var input = new BonInput(reader);

        return deserialize(input);
    }

    private async Task LoadBlock(uint blockId)
    {
        if (BlockIsLoaded(blockId))
        {
            return;
        }

        await _schemaStoreUpdater.UpdateSchemaStore().ConfigureAwait(false);

        if (BlockIsLoaded(blockId))
        {
            return;
        }

        throw new DeserializationFailedException(
            $"Cannot find the schemas that were used during serialization. " +
            $"Block {blockId} cannot be found.");
    }

    // Bookmark 553576978
    private bool BlockIsLoaded(uint blockId)
    {
        // Bookmark 697235293
        // A block ID of zero means there is no block.
        // This is possible when at the time of serialization there were no types with the BonObject attribute.
        if (blockId == 0)
        {
            return true;
        }

        return _blockStore.ContainsBlockId(blockId);
    }
}
