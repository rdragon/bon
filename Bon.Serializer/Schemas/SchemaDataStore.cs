namespace Bon.Serializer.Schemas;

internal sealed class SchemaDataStore(SchemaByTypeStore schemaByTypeStore)
{
    private readonly ConcurrentDictionary<Type, SchemaData> _schemaDatas = new();

    public SchemaData GetSchemaData(Type type)
    {
        if (!_schemaDatas.TryGetValue(type, out var schemaData))
        {
            schemaData = SchemaData.Create(schemaByTypeStore.GetSchemaByType(type));
            schemaData = _schemaDatas.TryAdd(type, schemaData) ? schemaData : _schemaDatas[type];
        }

        return schemaData;
    }
}
