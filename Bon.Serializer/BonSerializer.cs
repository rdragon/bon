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
    private readonly BlockStore _blockStore;
    private readonly SchemaDataResolver _schemaDataResolver;
    private readonly SchemaDataStore _schemaDataStore;
    private readonly WriterStore _writerStore;
    private readonly SimpleWriterStore _simpleWriterStore;
    private readonly SchemaStoreUpdater _schemaStoreUpdater;
    private readonly DeserializerStore _deserializerStore;

    private BonSerializer(
        BlockStore blockStore,
        SchemaDataResolver schemaDataResolver,
        SchemaDataStore schemaDataStore,
        WriterStore writerStore,
        SimpleWriterStore simpleWriterStore,
        SchemaStoreUpdater schemaStoreUpdater,
        DeserializerStore deserializerStore)
    {
        _blockStore = blockStore;
        _schemaDataResolver = schemaDataResolver;
        _schemaDataStore = schemaDataStore;
        _writerStore = writerStore;
        _simpleWriterStore = simpleWriterStore;
        _schemaStoreUpdater = schemaStoreUpdater;
        _deserializerStore = deserializerStore;
    }

    /// <summary>
    /// Creates a new <see cref="BonSerializer"/> instance.
    /// The serializer will contain the schemas found in <paramref name="bonSerializerContext"/> and <paramref name="schemasStorage"/>.
    /// During creation the <paramref name="schemasStorage"/> blob is updated with all the schemas found in
    /// <paramref name="bonSerializerContext"/>.
    /// </summary>
    /// <param name="bonSerializerContext"></param>
    /// <param name="schemasStorage">
    /// The blob that is used to save and load the schemas.
    /// The same blob must be used for serialization and deserialization.
    /// </param>
    public static async Task<BonSerializer> CreateAsync(IBonSerializerContext bonSerializerContext, IBlob schemasStorage)
    {
        ISourceGenerationContext sourceGenerationContext = bonSerializerContext.SourceGenerationContext;
        SchemaStorage schemaStorage = new(schemasStorage);
        SchemaContentsStore schemaContentsStore = new();
        SchemaByTypeStore schemaByTypeStore = new();
        SchemaDataResolver schemaDataResolver = new(schemaContentsStore);
        BlockStore blockStore = new();
        WriterStore writerStore = new();
        SimpleWriterStore simpleWriterStore = new();
        SchemaDataStore schemaDataStore = new(schemaByTypeStore);
        DeserializerStore deserializerStore = new(schemaByTypeStore);
        BonFacade bonFacade = new(schemaContentsStore, schemaByTypeStore, deserializerStore, writerStore);
        SchemaStoreUpdater schemaStoreUpdater = new(schemaStorage, blockStore, schemaContentsStore, schemaByTypeStore, schemaDataResolver, sourceGenerationContext, bonFacade);

        await schemaStoreUpdater.InitializeSchemaStore().ConfigureAwait(false);
        writerStore.AddBuiltInWriters();
        deserializerStore.AddNativeReaders();
        sourceGenerationContext.Run(bonFacade);
        simpleWriterStore.Initialize();

        return new BonSerializer(
            blockStore,
            schemaDataResolver,
            schemaDataStore,
            writerStore,
            simpleWriterStore,
            schemaStoreUpdater,
            deserializerStore);
    }

    /// <summary>
    /// Creates a new <see cref="BonSerializer"/> instance.
    /// The serializer will contain the schemas found in <paramref name="bonSerializerContext"/> and <paramref name="schemaStorageFile"/>.
    /// During creation the <paramref name="schemaStorageFile"/> is updated with all the schemas found in
    /// <paramref name="bonSerializerContext"/>.
    /// </summary>
    /// <param name="bonSerializerContext"></param>
    /// <param name="schemaStorageFile">
    /// The path to the file that is used to save and load the schemas.
    /// The same file must be used for serialization and deserialization.
    /// </param>
    public static Task<BonSerializer> CreateAsync(IBonSerializerContext bonSerializerContext, string schemaStorageFile) =>
        CreateAsync(bonSerializerContext, new FileSystemBlob(schemaStorageFile));

    internal int GetContentsId(Type type) => _schemaStoreUpdater.GetContentsId(type);
    internal uint LastBlockId => _blockStore.LastBlockId;
    internal int DeserializerCount => _deserializerStore.DeserializerCount;

    /// <summary>
    /// Returns a hash that is mostly based on the schemas inside this serializer.
    /// </summary>
    internal int GetSchemaHash()
    {
        var hashCode = new HashCode();
        _blockStore.AppendHash(ref hashCode);
        _schemaStoreUpdater.AppendHash(ref hashCode);

        return hashCode.ToHashCode();
    }
}
