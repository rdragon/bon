namespace Bon.Serializer.Schemas;

internal sealed class SchemaDataResolver(LayoutStore layoutStore)
{
    public Schema1 GetSchemaBySchemaData(SchemaData schemaData) =>
        TryGetSchemaBySchemaData(schemaData) ?? throw new SchemaException($"Can't find schema for '{schemaData}'.");

    /// <summary>
    /// Returns the schema corresponding to the input or null if for at least one <see cref="CustomSchemaData"/> no schema contents
    /// can be found in the <see cref="LayoutStore"/>.
    /// </summary>
    public Schema1? TryGetSchemaBySchemaData(SchemaData schemaData)
    {
        if (schemaData is CustomSchemaData customSchemaData)
        {
            return TryGetSchemaByCustomSchemaData(customSchemaData);
        }

        var innerSchemas = schemaData.InnerSchemas.Select(schema => TryGetSchemaBySchemaData(schema)!).ToArray();

        if (Array.Exists(innerSchemas, schema => schema is null))
        {
            return null;
        }

        return Schema1.CreateNonCustomSchema(schemaData.SchemaType, innerSchemas);
    }

    private CustomSchema? TryGetSchemaByCustomSchemaData(CustomSchemaData schemaData)
    {
        if (layoutStore.TryGet(schemaData.LayoutId, out var contents))
        {
            return CustomSchema.Create(schemaData.SchemaType, contents.Members, schemaData.LayoutId);
        }

        return null;
    }
}
