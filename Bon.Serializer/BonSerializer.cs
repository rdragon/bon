using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Bon.Serializer.Test")]
[assembly: InternalsVisibleTo("Bon.FileInspector")]
[assembly: InternalsVisibleTo("Bon.FileInspector.Test")]

namespace Bon.Serializer;

/// <summary>
/// Provides functionality to serialize objects to a binary format and to deserialize them back.
/// </summary>
public sealed partial class BonSerializer
{
    private readonly LayoutStore _layoutStore;
    private readonly WriterStore _writerStore;
    private readonly LayoutStorage _layoutStorage;
    private readonly DeserializerStore _deserializerStore;
    private readonly SchemaStore _schemaStore;

    private BonSerializer(
        LayoutStore layoutStore,
        WriterStore writerStore,
        LayoutStorage layoutStorage,
        DeserializerStore deserializerStore,
        SchemaStore schemaStore)
    {
        _layoutStore = layoutStore;
        _writerStore = writerStore;
        _layoutStorage = layoutStorage;
        _deserializerStore = deserializerStore;
        _schemaStore = schemaStore;
    }

    /// <summary>
    /// Creates a new <see cref="BonSerializer"/> instance.
    /// The serializer will contain the schemas found in <paramref name="bonSerializerContext"/> and <paramref name="blob"/>.
    /// During creation the <paramref name="blob"/> blob is updated with all the schemas found in
    /// <paramref name="bonSerializerContext"/>.
    /// </summary>
    /// <param name="bonSerializerContext"></param>
    /// <param name="blob">
    /// The blob that is used to save and load the schemas.
    /// The same blob must be used for serialization and deserialization.
    /// </param>
    public static async Task<BonSerializer> CreateAsync(IBonSerializerContext bonSerializerContext, IBlob blob)
    {
        ISourceGenerationContext sourceGenerationContext = bonSerializerContext.SourceGenerationContext;
        LayoutStore layoutStore = new();
        LayoutStorage layoutStorage = new(blob, layoutStore);
        SchemaStore schemaStore = new();
        WriterStore writerStore = new();
        DeserializerStore deserializerStore = new(schemaStore);
        BonFacade bonFacade = new(deserializerStore, writerStore);
        SchemaLoader storeUpdater = new(layoutStorage, layoutStore, schemaStore, sourceGenerationContext);

        await storeUpdater.Run().ConfigureAwait(false);
        writerStore.AddNativeWriters();
        deserializerStore.AddNativeReaders();
        sourceGenerationContext.Run(bonFacade);

        return new BonSerializer(
            layoutStore,
            writerStore,
            layoutStorage,
            deserializerStore,
            schemaStore);
    }

    /// <summary>
    /// Creates a new <see cref="BonSerializer"/> instance.
    /// The serializer will contain the schemas found in <paramref name="bonSerializerContext"/> and <paramref name="layoutStorageFile"/>.
    /// During creation the <paramref name="layoutStorageFile"/> is updated with all the schemas found in
    /// <paramref name="bonSerializerContext"/>.
    /// </summary>
    /// <param name="bonSerializerContext"></param>
    /// <param name="layoutStorageFile">
    /// The path to the file that is used to save and load the schemas.
    /// The same file must be used for serialization and deserialization.
    /// </param>
    public static Task<BonSerializer> CreateAsync(IBonSerializerContext bonSerializerContext, string layoutStorageFile) =>
        CreateAsync(bonSerializerContext, new FileSystemBlob(layoutStorageFile));

    /// <summary>
    /// //2at
    /// </summary>
    public async Task LoadLatestLayouts()
    {
        await _layoutStorage.LoadLatestLayouts().ConfigureAwait(false);
    }

    public string PrintKnownLayouts()
    {
        return new FullSchemaPrinter().Print(_layoutStore.Layouts);
    }

    public string PrintKnownSchemas(Predicate<Schema>? filter = null)
    {
        var schemas = _schemaStore.Schemas.Where(pair => filter is null || filter(pair.Value));
        return new FullSchemaPrinter().Print(schemas);
    }

    public static string PrintLayouts(byte[] blob)
    {
        var store = new LayoutStore();
        var reader = new LayoutReader(store, new BinaryReader(new MemoryStream(blob)), true);
        reader.ReadManyLayouts();
        return new FullSchemaPrinter().Print(store.Layouts);
    }

    //2at
    internal Schema GetSchema(Type type) => _schemaStore.GetOrAddSchema(type);

    //2at
    internal int DeserializerCount => _deserializerStore.DeserializerCount;

    internal IEnumerable<Layout> KnownLayouts => _layoutStore.Layouts;
}
