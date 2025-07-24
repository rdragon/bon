namespace Bon.Serializer.Schemas;

internal sealed class SchemaDataStore(SchemaStore schemaStore)
{
    private readonly ConcurrentDictionary<Type, SchemaData> _schemaDatas = new();

    public SchemaData GetSchemaData(Type type)
    {
        if (!_schemaDatas.TryGetValue(type, out var schemaData))
        {
            schemaData = SchemaData.Create(schemaStore.GetOrAdd(type));
            schemaData = _schemaDatas.TryAdd(type, schemaData) ? schemaData : _schemaDatas[type];
        }

        return schemaData;
    }
}
