namespace Bon.Serializer;

public partial class BonSerializer
{
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
            FormatType.Byte => new SchemaData(SchemaType.Byte, []),
            FormatType.SByte => new SchemaData(SchemaType.SByte, []),
            FormatType.Short => new SchemaData(SchemaType.Short, []),
            FormatType.UShort => new SchemaData(SchemaType.UShort, []),
            FormatType.Int => new SchemaData(SchemaType.Int, []),
            FormatType.UInt => new SchemaData(SchemaType.UInt, []),
            FormatType.Long => new SchemaData(SchemaType.Long, []),
            FormatType.ULong => new SchemaData(SchemaType.ULong, []),
            _ => throw new DeserializationFailedException($"Cannot handle format type {formatType}.")
        };
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

    private readonly record struct Header(FormatType FormatType, uint BlockId, SchemaData SchemaData);
}
