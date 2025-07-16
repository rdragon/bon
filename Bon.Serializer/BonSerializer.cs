using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Bon.Serializer.Test")]

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

    /// <summary>
    /// Whether during the creation of the serializer new layouts were found and saved to the blob.
    /// </summary>
    public bool FoundNewLayouts { get; }

    private BonSerializer(
        LayoutStore layoutStore,
        WriterStore writerStore,
        LayoutStorage layoutStorage,
        DeserializerStore deserializerStore,
        SchemaStore schemaStore,
        bool foundNewLayouts)
    {
        _layoutStore = layoutStore;
        _writerStore = writerStore;
        _layoutStorage = layoutStorage;
        _deserializerStore = deserializerStore;
        _schemaStore = schemaStore;
        FoundNewLayouts = foundNewLayouts;
    }

    /// <summary>
    /// Creates a new <see cref="BonSerializer"/> instance.
    /// This method loads the layouts from the provided blob and updates the blob if any new layouts were found by
    /// the source generation context.
    /// </summary>
    /// <param name="bonSerializerContext">
    /// The context that contains information about the types that can be serialized and deserialized.
    /// </param>
    /// <param name="blob">
    /// The blob that is used to keep track of the known layouts.
    /// </param>
    public static async Task<BonSerializer> CreateAsync(IBonSerializerContext bonSerializerContext, IBlob blob)
    {
        LayoutStore layoutStore = new();
        LayoutStorage layoutStorage = new(blob, layoutStore);
        SchemaStore schemaStore = new();
        WriterStore writerStore = new();
        DeserializerStore deserializerStore = new(schemaStore);
        BonFacade bonFacade = new(deserializerStore, writerStore);
        SchemaLoader schemaLoader = new(layoutStorage, layoutStore, schemaStore, bonSerializerContext.SourceGenerationContext);

        var foundNewLayouts = await schemaLoader.RunAsync().ConfigureAwait(false);
        writerStore.AddNativeWriters();
        deserializerStore.AddNativeReaders();
        bonSerializerContext.SourceGenerationContext.Run(bonFacade);

        return new BonSerializer(
            layoutStore,
            writerStore,
            layoutStorage,
            deserializerStore,
            schemaStore,
            foundNewLayouts);
    }

    /// <summary>
    /// Loads the latest layouts from the storage.
    /// If the entity tag hasn't changed, the blob contents are not downloaded.
    /// </summary>
    public async Task LoadLatestLayoutsAsync()
    {
        await _layoutStorage.LoadLatestLayoutsAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// The number of layouts that are known.
    /// </summary>
    public int LayoutCount => _layoutStore.Count;

    /// <summary>
    /// Returns the schema for the provided type.
    /// </summary>
    internal Schema GetSchema(Type type) => _schemaStore.GetOrAddSchema(type);

    /// <summary>
    /// The number of cached deserializers.
    /// </summary>
    internal int DeserializerCount => _deserializerStore.DeserializerCount;

    internal IEnumerable<Layout> KnownLayouts => _layoutStore.Layouts;
}
