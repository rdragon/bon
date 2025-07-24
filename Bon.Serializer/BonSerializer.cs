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
    private readonly SchemaDataResolver _schemaDataResolver;
    private readonly SchemaDataStore _schemaDataStore;
    private readonly WriterStore _writerStore;
    private readonly SimpleWriterStore _simpleWriterStore;
    private readonly StoreUpdater _layoutStoreUpdater;
    private readonly DeserializerStore _deserializerStore;

    private BonSerializer(
        LayoutStore layoutStore,
        SchemaDataResolver schemaDataResolver,
        SchemaDataStore schemaDataStore,
        WriterStore writerStore,
        SimpleWriterStore simpleWriterStore,
        StoreUpdater layoutStoreUpdater,
        DeserializerStore deserializerStore)
    {
        _layoutStore = layoutStore;
        _schemaDataResolver = schemaDataResolver;
        _schemaDataStore = schemaDataStore;
        _writerStore = writerStore;
        _simpleWriterStore = simpleWriterStore;
        _layoutStoreUpdater = layoutStoreUpdater;
        _deserializerStore = deserializerStore;
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
        SchemaDataResolver schemaDataResolver = new(layoutStore);
        WriterStore writerStore = new();
        SimpleWriterStore simpleWriterStore = new();
        SchemaDataStore schemaDataStore = new(schemaStore);
        DeserializerStore deserializerStore = new(schemaStore);
        BonFacade bonFacade = new(layoutStore, schemaStore, deserializerStore, writerStore);
        StoreUpdater layoutStoreUpdater = new(layoutStorage, layoutStore, schemaStore, sourceGenerationContext, bonFacade);

        await layoutStoreUpdater.InitializeStores().ConfigureAwait(false);
        writerStore.AddNativeWriters();
        deserializerStore.AddNativeReaders();
        sourceGenerationContext.Run(bonFacade);
        simpleWriterStore.Initialize();

        return new BonSerializer(
            layoutStore,
            schemaDataResolver,
            schemaDataStore,
            writerStore,
            simpleWriterStore,
            layoutStoreUpdater,
            deserializerStore);
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

    internal int GetLayoutId(Type type) => _layoutStoreUpdater.GetLayoutId(type);
    internal int DeserializerCount => _deserializerStore.DeserializerCount;

    /// <summary>
    /// Returns a hash that is mostly based on the schemas inside this serializer.
    /// </summary>
    internal int GetSchemaHash()
    {
        var hashCode = new HashCode();
        _layoutStore.AppendHash(ref hashCode);
        _layoutStoreUpdater.AppendHash(ref hashCode);

        return hashCode.ToHashCode();
    }

    /// <summary>
    /// //2at
    /// </summary>
    public async Task LoadLatestSchemas()
    {
        await _layoutStoreUpdater.UpdateLayoutStore().ConfigureAwait(false);
    }
}
