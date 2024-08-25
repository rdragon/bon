namespace Bon.Serializer;

public sealed class BonFacade
{
    private readonly SchemaContentsStore _schemaContentsStore;
    private readonly SchemaByTypeStore _schemaByTypeStore;
    private readonly DeserializerStore _deserializerStore;
    private readonly WriterStore _writerStore;
    private readonly DefaultValueGetterFactory _defaultValueGetterFactory;

    internal BonFacade(
        SchemaContentsStore schemaContentsStore,
        SchemaByTypeStore schemaByTypeStore,
        DeserializerStore deserializerStore,
        WriterStore writerStore,
        DefaultValueGetterFactory defaultValueGetterFactory)
    {
        _schemaContentsStore = schemaContentsStore;
        _schemaByTypeStore = schemaByTypeStore;
        _deserializerStore = deserializerStore;
        _writerStore = writerStore;
        _defaultValueGetterFactory = defaultValueGetterFactory;
    }

    public void AddSchema(Type type, Schema schema)
    {
        if (_schemaByTypeStore.TryAdd(type, schema) && schema is CustomSchema customSchema)
        {
            var contentsId = _schemaContentsStore.GetOrAddContentsId(customSchema.Contents);
            customSchema.ContentsId = contentsId;
        }
    }

    public void AddWriter<T>(Action<BinaryWriter, T> writer) => _writerStore.Add(writer);

    public void AddEnumData(Type type, Type underlyingType, Func<Delegate, Delegate> addEnumCast) =>
        _deserializerStore.AddEnumData(type, underlyingType, addEnumCast);

    public void AddReaderFactory(Type type, Delegate factory) => _deserializerStore.AddReaderFactory(type, factory);

    public void AddMemberType(Type unionType, int memberId, Type memberType) =>
        _deserializerStore.AddMemberType(unionType, memberId, memberType);

    public void AddDefaultValueGetter(Type type, Delegate getDefaultValue) =>
        _defaultValueGetterFactory.AddDefaultValueGetter(type, getDefaultValue);

    public void AddDeserializer(Type type, bool isNullable, Delegate deserializer) =>
        _deserializerStore.Add(type, isNullable, deserializer);
}
