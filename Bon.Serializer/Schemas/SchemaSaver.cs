namespace Bon.Serializer.Schemas;

internal sealed class SchemaSaver(BlockStore blockStore, SchemaStorage schemaStorage, SchemaContentsStore schemaContentsStore)
{
    public async Task<bool> SaveNewestSchemas(int count)
    {
        if (count == 0)
        {
            return true;
        }

        var schemas = schemaContentsStore.GetNewestSchemas(count).ToArray();
        var blockId = GetNewBlockId();
        var block = new Block(blockId, schemas);

        if (await schemaStorage.TryAppend(block, blockStore.EntityTag).ConfigureAwait(false) is not EntityTag entityTag)
        {
            return false;
        }

        blockStore.LastBlockId = blockId;
        blockStore.EntityTag = entityTag;
        blockStore.AddBlockId(blockId);

        return true;
    }

    private uint GetNewBlockId()
    {
        for (var i = 0; i < 1000; i++)
        {
            var blockId = BonHelper.GetRandomUInt();

            // Zero is not a valid block ID.
            if (blockId == 0)
            {
                continue;
            }

            // It is important that the block ID is unique because of the code at bookmark 553576978.
            if (!blockStore.ContainsBlockId(blockId))
            {
                return blockId;
            }
        }

        throw new InvalidOperationException("This should not happen.");
    }
}
