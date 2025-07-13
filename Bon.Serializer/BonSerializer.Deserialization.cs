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
        var header = await ReadHeaderAsync(reader).ConfigureAwait(false);

        return DeserializeBody<T>(reader, header.SchemaData);
    }

    /// <summary>
    /// Deserializes a value from a stream.
    /// If unknown schema IDs are encountered then false is returned and the out parameter is set to the default value.
    /// </summary>
    public bool TryDeserialize<T>(Stream stream, [MaybeNullWhen(false)] out T value)
    {
        var reader = new BinaryReader(stream);

        if (TryReadHeader(reader, out var header))
        {
            value = DeserializeBody<T>(reader, header.SchemaData);
            return true;
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Deserializes a value from a stream.
    /// If unknown schema IDs are encountered then an exception is thrown.
    /// </summary>
    public T Deserialize<T>(Stream stream)
    {
        var reader = new BinaryReader(stream);
        var header = ReadHeader(reader);
        return DeserializeBody<T>(reader, header.SchemaData);
    }

    /// <summary>
    /// Deserializes a value from a stream and then converts the value to a <see cref="JsonObject"/>.
    /// Loads new schemas from the storage if unknown schema IDs are encountered.
    /// </summary>
    public async Task<JsonObject> BonToJsonAsync(Stream stream)
    {
        var reader = new BinaryReader(stream);
        var (formatType, blockId, schemaData) = await ReadHeaderAsync(reader).ConfigureAwait(false);
        var schemaDataBytes = SchemaSerializer.Write(schemaData);
        var schema = _schemaDataResolver.GetSchemaBySchemaData(schemaData);

        return new JsonObject
        {
            ["body"] = BonToJsonDeserializer.Deserialize(reader, schema),
            ["schema"] = Convert.ToBase64String(schemaDataBytes),
            ["blockId"] = blockId,
        };
    }

    /// <summary>
    /// Deserializes a value from a stream and then converts the value to a JSON string.
    /// Loads new schemas from the storage if unknown schema IDs are encountered.
    /// </summary>
    public async Task<string> BonToJsonAsync(byte[] bytes)
    {
        var stream = new MemoryStream(bytes);
        var jsonObject = await BonToJsonAsync(stream).ConfigureAwait(false);
        return jsonObject.ToJsonString();
    }

    private async Task<Header> ReadHeaderAsync(BinaryReader reader)
    {
        var headerPart = ReadFirstPartOfHeader(reader);
        if (TryReadHeader(reader, headerPart, out var header))
        {
            return header;
        }

        await LoadBlock(headerPart.BlockId).ConfigureAwait(false);
        return ReadHeader(reader, headerPart);
    }

    private Header ReadHeader(BinaryReader reader)
    {
        var headerPart = ReadFirstPartOfHeader(reader);
        if (TryReadHeader(reader, headerPart, out var header))
        {
            return header;
        }

        throw GetBlockNotFoundException(headerPart.BlockId);
    }

    private bool TryReadHeader(BinaryReader reader, out Header header)
    {
        var headerPart = ReadFirstPartOfHeader(reader);
        return TryReadHeader(reader, headerPart, out header);
    }

    private static HeaderPart ReadFirstPartOfHeader(BinaryReader reader)
    {
        var formatType = (FormatType)reader.ReadByte();
        var blockId = formatType == FormatType.Full ? reader.ReadUInt32() : 0;
        return new(formatType, blockId);
    }

    private bool TryReadHeader(BinaryReader reader, HeaderPart headerPart, out Header header)
    {
        var (_, blockId) = headerPart;

        if (blockId != 0 && !BlockIsLoaded(blockId))
        {
            header = default;
            return false;
        }

        header = ReadHeader(reader, headerPart);
        return true;
    }

    private static Header ReadHeader(BinaryReader reader, HeaderPart headerPart)
    {
        var (formatType, blockId) = headerPart;
        var schemaData = ReadSchemaData(reader, formatType);

        return new(formatType, blockId, schemaData);
    }

    private static SchemaData ReadSchemaData(BinaryReader reader, FormatType formatType)
    {
        return formatType switch
        {
            FormatType.Full or FormatType.WithoutBlockId => SchemaSerializer.ReadSchemaData(reader),
            FormatType.Byte => new SchemaData(SchemaType.Byte, false, []),
            FormatType.SByte => new SchemaData(SchemaType.SByte, false, []),
            FormatType.Short => new SchemaData(SchemaType.Short, false, []),
            FormatType.UShort => new SchemaData(SchemaType.UShort, false, []),
            FormatType.Int => new SchemaData(SchemaType.Int, false, []),
            FormatType.UInt => new SchemaData(SchemaType.UInt, false, []),
            FormatType.Long => new SchemaData(SchemaType.Long, false, []),
            FormatType.ULong => new SchemaData(SchemaType.ULong, false, []),
            _ => throw new DeserializationFailedException($"Cannot handle format type {formatType}.")
        };
    }

    private T DeserializeBody<T>(BinaryReader reader, SchemaData schemaData)
    {
        var schema = _schemaDataResolver.GetSchemaBySchemaData(schemaData);
        var deserialize = _deserializerStore.GetDeserializer<T>(schema, null);
        var input = new BonInput(reader);

        return deserialize(input);
    }

    private async Task LoadBlock(uint blockId)
    {
        if (blockId == 0 || BlockIsLoaded(blockId))
        {
            return;
        }

        await _schemaStoreUpdater.UpdateSchemaStore().ConfigureAwait(false);

        if (!BlockIsLoaded(blockId))
        {
            throw GetBlockNotFoundException(blockId);
        }
    }

    private static DeserializationFailedException GetBlockNotFoundException(uint blockId) => new(
        $"Cannot find the schemas that were used during serialization. " +
        $"Block {blockId} cannot be found.");

    // Bookmark 553576978
    private bool BlockIsLoaded(uint blockId)
    {
        return _blockStore.ContainsBlockId(blockId);
    }

    private readonly record struct HeaderPart(FormatType FormatType, uint BlockId);

    internal readonly record struct Header(FormatType FormatType, uint BlockId, SchemaData SchemaData);
}
