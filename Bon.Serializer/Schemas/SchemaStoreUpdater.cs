namespace Bon.Serializer.Schemas;

internal sealed class SchemaStoreUpdater(
     SchemaStorage schemaStorage,
     BlockStore blockStore,
     SchemaContentsStore schemaContentsStore,
     SchemaByTypeStore schemaByTypeStore,
     SchemaDataResolver schemaDataResolver,
     ISourceGenerationContext sourceGenerationContext,
     BonFacade bonFacade)
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    /// <summary>
    /// Adds the native schemas, the schemas from the storage and the schemas from the source generation context to the store.
    /// Then saves the schemas from the source generation context to the storage.
    /// </summary>
    public async Task InitializeSchemaStore()
    {
        const int MAX_ATTEMPTS = 3;

        for (int i = 0; i < MAX_ATTEMPTS; i++)
        {
            if (await InitializeSchemaStoreNow().ConfigureAwait(false))
            {
                return;
            }

            ClearStore();
        }

        throw new IOException("Failed to save the new schemas to the storage.");
    }

    private async Task<bool> InitializeSchemaStoreNow()
    {
        schemaByTypeStore.AddNativeSchemas();
        await AddSchemasFromStorage().ConfigureAwait(false);
        var count = AddSchemasFromSourceGenerationContext();

        return await SaveNewSchemasToStorage(count).ConfigureAwait(false);
    }

    /// <summary>
    /// Adds all schemas from the storage to the store.
    /// </summary>
    public async Task UpdateSchemaStore()
    {
        var entityTag = await schemaStorage.GetEntityTag().ConfigureAwait(false);

        // If the entity tag is the same the schema store is already up to date.
        if (entityTag == blockStore.EntityTag)
        {
            return;
        }

        await _semaphore.WaitAsync().ConfigureAwait(false);

        try
        {
            if (entityTag == blockStore.EntityTag)
            {
                return;
            }

            await AddSchemasFromStorage().ConfigureAwait(false);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task AddSchemasFromStorage()
    {
        var schemaContentsStoreUpdater = new SchemaContentsStoreUpdater(schemaContentsStore, schemaDataResolver);

        var (blocks, entityTag) = await schemaStorage.Load().ConfigureAwait(false);

        foreach (var block in blocks)
        {
            schemaContentsStoreUpdater.Add(block);
        }

        foreach (var block in blocks)
        {
            blockStore.AddBlockId(block.BlockId);
        }

        if (blocks.Count > 0)
        {
            blockStore.LastBlockId = blocks[^1].BlockId;
        }

        blockStore.EntityTag = entityTag;
    }

    private int AddSchemasFromSourceGenerationContext()
    {
        var countBefore = schemaContentsStore.Count;
        sourceGenerationContext.UpdateSchemaStore(bonFacade);

        return schemaContentsStore.Count - countBefore;
    }

    private Task<bool> SaveNewSchemasToStorage(int count) =>
        new SchemaSaver(blockStore, schemaStorage, schemaContentsStore).SaveNewestSchemas(count);

    private void ClearStore()
    {
        schemaContentsStore.Clear();
        schemaByTypeStore.Clear();
        blockStore.Clear();
    }

    public void AppendHash(ref HashCode hashCode)
    {
        schemaContentsStore.AppendHash(ref hashCode);
        schemaByTypeStore.AppendHash(ref hashCode);
    }

    public int GetContentsId(Type type) => ((CustomSchema)schemaByTypeStore.GetSchemaByType(type)).ContentsId;
}
