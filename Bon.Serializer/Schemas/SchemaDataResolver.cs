namespace Bon.Serializer.Schemas;

internal sealed class SchemaDataResolver(SchemaContentsStore schemaContentsStore)
{
    public Schema GetSchemaBySchemaData(SchemaData schemaData) =>
        TryGetSchemaBySchemaData(schemaData) ?? throw new SchemaException($"Can't find schema for '{schemaData}'.");

    /// <summary>
    /// Returns the schema corresponding to the input or null if for at least one <see cref="CustomSchemaData"/> no schema contents
    /// can be found in the <see cref="SchemaContentsStore"/>.
    /// </summary>
    public Schema? TryGetSchemaBySchemaData(SchemaData schemaData)
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

        return Schema.CreateNonCustomSchema(schemaData.SchemaType, innerSchemas);
    }

    private CustomSchema? TryGetSchemaByCustomSchemaData(CustomSchemaData schemaData)
    {
        if (schemaContentsStore.TryGet(schemaData.ContentsId, out var contents))
        {
            return CustomSchema.Create(schemaData.SchemaType, contents.Members, schemaData.ContentsId);
        }

        return null;
    }
}
