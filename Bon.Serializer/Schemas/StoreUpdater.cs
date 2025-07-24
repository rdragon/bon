namespace Bon.Serializer.Schemas;

internal sealed class StoreUpdater(
     LayoutStorage layoutStorage,
     LayoutStore layoutStore,
     SchemaStore schemaStore,
     ISourceGenerationContext sourceGenerationContext,
     BonFacade bonFacade)
{
    /// <summary>
    /// Adds the native schemas, the layouts from the storage and the schemas and layouts from the source generation context to the stores.
    /// Then saves the new layouts to the storage.
    /// </summary>
    public async Task InitializeStores()
    {
        const int MAX_ATTEMPTS = 3;

        for (int i = 0; i < MAX_ATTEMPTS; i++)
        {
            if (await InitializeStoresNow().ConfigureAwait(false))
            {
                return;
            }

            ClearStores();
        }

        throw new IOException("Failed to save the new layouts to the storage.");
    }

    private async Task<bool> InitializeStoresNow()
    {
        schemaStore.AddNativeSchemas();
        await AddLayoutsFromStorage().ConfigureAwait(false);
        AddFromSourceGenerationContext();

        return await SaveNewLayoutsToStorage().ConfigureAwait(false);
    }

    /// <summary>
    /// Adds all schemas from the storage to the store.
    /// </summary>
    public async Task UpdateLayoutStore()
    {
        await layoutStorage.LoadIfNecessary().ConfigureAwait(false);
    }

    private async Task AddLayoutsFromStorage()
    {
        await layoutStorage.Load().ConfigureAwait(false);
    }

    private void AddFromSourceGenerationContext()
    {
        sourceGenerationContext.UpdateSchemaStore(bonFacade);
    }

    private async Task<bool> SaveNewLayoutsToStorage()
    {
        var layouts = layoutStore.PopLayoutsToSave();

        return await layoutStorage.TrySave(layouts).ConfigureAwait(false);
    }

    private void ClearStores()
    {
        layoutStore.Clear();
        schemaStore.Clear();
    }

    public void AppendHash(ref HashCode hashCode)
    {
        layoutStore.AppendHash(ref hashCode);
        schemaStore.AppendHash(ref hashCode);
    }

    public int GetLayoutId(Type type) => ((CustomSchema)schemaStore.GetOrAdd(type)).LayoutId;
}
