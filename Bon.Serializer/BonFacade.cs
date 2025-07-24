namespace Bon.Serializer;

/// <summary>
/// //2at
/// </summary>
public sealed class BonFacade
{
    private readonly LayoutStore _layoutStore;
    private readonly SchemaStore _schemaStore;
    private readonly DeserializerStore _deserializerStore;
    private readonly WriterStore _writerStore;

    internal BonFacade(
        LayoutStore layoutStore,
        SchemaStore schemaStore,
        DeserializerStore deserializerStore,
        WriterStore writerStore)
    {
        _layoutStore = layoutStore;
        _schemaStore = schemaStore;
        _deserializerStore = deserializerStore;
        _writerStore = writerStore;
    }

    public void AddSchema(Type type, Schema schema)
    {
        _schemaStore.AddSchema(type, schema);

        if (schema.IsCustomSchema)
        {
            _layoutStore.Add(schema);
        }
    }

    public void AddWriter(Type type, Delegate writer, bool usesCustomSchemas) => _writerStore.Add(type, writer, usesCustomSchemas);

    public void AddWeakDeserializerFactory<T1, T2>(Func<T2, T1> func) => _deserializerStore.AddWeakDeserializerFactory(func);

    public void AddReaderFactory(Type type, Delegate factory) => _deserializerStore.AddReaderFactory(type, factory);

    public void AddMemberType(Type unionType, int memberId, Type memberType) =>
        _deserializerStore.MemberTypes[(unionType, memberId)] = memberType;

    public void AddDeserializer(Type type, Delegate deserializer) =>
        _deserializerStore.AddDeserializer(type, deserializer);
}
