namespace Bon.Serializer.Schemas;

/// <summary>
/// Fills the stores with schemas and layouts.
/// This class is only used during the creation of a BonSerializer instance.
/// </summary>
internal sealed class SchemaLoader(
     LayoutStorage layoutStorage,
     LayoutStore layoutStore,
     SchemaStore schemaStore,
     ISourceGenerationContext sourceGenerationContext)
{
    /// <summary>
    /// Keeps track of all known layout IDs.
    /// </summary>
    private readonly Dictionary<Structure, int> _layoutIds = [];

    /// <summary>
    /// The layouts that need to be saved to the storage.
    /// These are the new layouts created by the source generation context.
    /// </summary>
    private readonly List<Layout> _layoutsToSave = [];

    /// <summary>
    /// The schemas that need to receive a "real" layout ID.
    /// This list eventually contains all the custom schemas from the source generation context.
    /// The reason we postpone updating the schemas is that the layout IDs are used to spot recursive
    /// schemas and there should not be a mix of "fake" and "real" layout IDs.
    /// </summary>
    private readonly List<(Schema Schema, int LayoutId)> _schemasToUpdate = [];

    /// <summary>
    /// Fills the stores with schemas and layouts.
    /// First adds the native schemas.
    /// Then loads the layouts from the storage.
    /// Then adds the schemas from the source generation context.
    /// For each custom schema that does not have a corresponding layout, a new layout is created.
    /// Then saves these new layouts to the storage.
    /// </summary>
    /// <returns>
    /// Whether new layouts were found and saved to the storage.
    /// </returns>
    public async Task<bool> RunAsync()
    {
        const int MAX_ATTEMPTS = 3;

        for (int i = 0; i < MAX_ATTEMPTS; i++)
        {
            if (await RunNowAsync().ConfigureAwait(false))
            {
                return _layoutsToSave.Count > 0;
            }

            ClearAll();
        }

        throw new IOException("Failed to save the new layouts to the storage.");
    }

    private async Task<bool> RunNowAsync()
    {
        schemaStore.AddNativeSchemas();
        await layoutStorage.LoadLayoutsAsync().ConfigureAwait(false);
        FillLayoutIds();
        sourceGenerationContext.LoadSchemas(OnSchemaLoaded);
        UpdateSchemas();

        if (_layoutsToSave.Count == 0)
        {
            return true;
        }

        return await layoutStorage.TrySaveAsync(_layoutsToSave).ConfigureAwait(false);
    }

    private void FillLayoutIds()
    {
        foreach (var layout in layoutStore.Layouts)
        {
            _layoutIds.Add(StructureFactory.Create(layout), layout.Id);
        }
    }

    private void UpdateSchemas()
    {
        foreach (var (schema, layoutId) in _schemasToUpdate)
        {
            schema.LayoutId = layoutId;
        }
    }

    /// <summary>
    /// Called by the source generation context for every record, union and enum.
    /// </summary>
    /// <param name="schema">The schema corresponding to the type. The schama contains "fake" layout IDs.</param>
    public void OnSchemaLoaded(Type type, Schema schema)
    {
        schemaStore.AddSchema(type, schema);

        if (!schema.IsCustom)
        {
            return;
        }

        Trace.Assert(schema.LayoutId < 0, "Expecting a negative Layout ID"); // See bookmark 458282233.
        var layoutId = GetOrCreateLayout(schema);
        _schemasToUpdate.Add((schema, layoutId));
    }

    /// <summary>
    /// Returns the ID of the layout corresponding to a custom schema.
    /// If the layout does not exist, it is created and added to the layout store.
    /// </summary>
    /// <param name="schema">A schema containing "fake" layout IDs.</param>
    private int GetOrCreateLayout(Schema schema)
    {
        var structure = StructureFactory.Create(schema, false);

        if (_layoutIds.TryGetValue(structure, out var id))
        {
            return id;
        }

        var layout = layoutStore.CreateLayout(schema.Members);
        _layoutIds[structure] = layout.Id;
        _layoutsToSave.Add(layout);
        return layout.Id;
    }

    private void ClearAll()
    {
        layoutStore.Clear();
        schemaStore.Clear();
        layoutStorage.Clear();
        _layoutIds.Clear();
        _layoutsToSave.Clear();
        _schemasToUpdate.Clear();
    }
}
