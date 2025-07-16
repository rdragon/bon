namespace Bon.Serializer;

public sealed class BonFacade
{
    private readonly SchemaContentsStore _schemaContentsStore;
    private readonly SchemaByTypeStore _schemaByTypeStore;
    private readonly DeserializerStore _deserializerStore;
    private readonly WriterStore _writerStore;

    internal BonFacade(
        SchemaContentsStore schemaContentsStore,
        SchemaByTypeStore schemaByTypeStore,
        DeserializerStore deserializerStore,
        WriterStore writerStore)
    {
        _schemaContentsStore = schemaContentsStore;
        _schemaByTypeStore = schemaByTypeStore;
        _deserializerStore = deserializerStore;
        _writerStore = writerStore;
    }

    public void AddSchema(Type type, Schema schema)
    {
        if (_schemaByTypeStore.TryAdd(type, schema) && schema is CustomSchema customSchema)
        {
            var contentsId = _schemaContentsStore.GetOrAddContentsId(customSchema.Contents);
            customSchema.ContentsId = contentsId;
        }
    }

    public void AddWriter<T>(Action<BinaryWriter, T> writeValue, bool usesCustomSchemas) => _writerStore.Add(writeValue, usesCustomSchemas);

    public void AddEnumData(Type type, Type underlyingType, Func<Delegate, Delegate> addEnumCast) =>
        _deserializerStore.AddEnumData(type, underlyingType, addEnumCast);

    public void AddReaderFactory(Type type, Delegate factory) => _deserializerStore.AddReaderFactory(type, factory);

    public void AddMemberType(Type unionType, int memberId, Type memberType) =>
        _deserializerStore.AddMemberType(unionType, memberId, memberType);

    public void AddDeserializer(Type type, Delegate deserializer) =>
        _deserializerStore.Add(type, deserializer);
}
