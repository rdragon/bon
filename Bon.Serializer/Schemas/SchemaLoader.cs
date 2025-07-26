namespace Bon.Serializer.Schemas;

//2at
internal sealed class SchemaLoader(
     LayoutStorage layoutStorage,
     LayoutStore layoutStore,
     SchemaStore schemaStore,
     ISourceGenerationContext sourceGenerationContext)
{
    private readonly Dictionary<Structure, int> _layoutIds = [];

    private readonly List<Layout> _layoutsToSave = [];

    /// <summary>
    /// Adds the native schemas, the layouts from the storage and the schemas and layouts from the source generation context to the stores.
    /// Then saves the new layouts to the storage.
    /// </summary>
    public async Task Run()
    {
        const int MAX_ATTEMPTS = 3;

        for (int i = 0; i < MAX_ATTEMPTS; i++)
        {
            if (await RunNow().ConfigureAwait(false))
            {
                return;
            }

            ClearAll();
        }

        throw new IOException("Failed to save the new layouts to the storage.");
    }

    private async Task<bool> RunNow()
    {
        schemaStore.AddNativeSchemas();
        await layoutStorage.LoadLayouts().ConfigureAwait(false);
        FillLayoutIds();
        sourceGenerationContext.LoadSchemas(OnSchemaLoaded);
        return await layoutStorage.TrySave(_layoutsToSave).ConfigureAwait(false);
    }

    private void FillLayoutIds()
    {
        foreach (var layout in layoutStore.Layouts)
        {
            _layoutIds.Add(new Structure(layout), layout.Id);
        }
    }

    //2at, source generated context
    public void OnSchemaLoaded(Type type, Schema schema)
    {
        schemaStore.AddSchema(type, schema);

        if (!schema.IsCustom)
        {
            return;
        }

        Trace.Assert(schema.LayoutId == 0, "Layout ID is non-zero");
        var layoutId = GetOrCreateLayout(schema);
        schema.LayoutId = layoutId;
    }

    //2at, source generated context
    private int GetOrCreateLayout(Schema schema)
    {
        var key = new Structure(schema);

        if (_layoutIds.TryGetValue(key, out var id))
        {
            return id;
        }

        var layout = layoutStore.CreateLayout(schema.Members);
        _layoutIds[key] = layout.Id;
        _layoutsToSave.Add(layout);
        return layout.Id;
    }

    private void ClearAll()
    {
        layoutStore.Clear();
        schemaStore.Clear();
        _layoutIds.Clear();
        _layoutsToSave.Clear();
        layoutStorage.Clear();
    }
}
