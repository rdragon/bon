namespace Bon.Serializer.Schemas;

internal static class BlockSerializer
{
    private const ushort BLOCK_MARKER = 0xad5d;
    private const byte VERSION = 0;

    public static void Serialize(Stream stream, Block block)
    {
        var writer = new BinaryWriter(stream);

        writer.Write(BLOCK_MARKER);
        writer.Write(VERSION);
        writer.Write(block.BlockId);
        WholeNumberSerializer.Write(writer, block.Schemas.Count);

        foreach (var schema in block.Schemas)
        {
            SchemaSerializer.Write(writer, schema);
        }
    }

    public static IEnumerable<Block> Deserialize(Stream stream)
    {
        var reader = new BinaryReader(stream);

        while (stream.Position < stream.Length)
        {
            ReadMarker(reader);
            ReadVersion(reader);
            var blockId = reader.ReadUInt32();
            var count = (int)WholeNumberSerializer.Read(reader);
            var schemas = new List<SchemaContentsData>(count);

            for (var i = 0; i < count; i++)
            {
                schemas.Add(SchemaSerializer.ReadSchema(reader));
            }

            yield return new Block(blockId, schemas);
        }
    }

    private static void ReadMarker(BinaryReader reader)
    {
        var marker = reader.ReadUInt16();

        if (marker != BLOCK_MARKER)
        {
            throw new DeserializationFailedException($"Invalid schema blob. Expected marker {BLOCK_MARKER:x4} but found {marker:x4}.");
        }
    }

    private static void ReadVersion(BinaryReader reader)
    {
        var version = reader.ReadByte();

        if (version > 0)
        {
            throw new DeserializationFailedException($"Invalid schema blob. Cannot handle block version {version}.");
        }
    }
}
