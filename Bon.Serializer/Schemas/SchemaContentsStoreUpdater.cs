namespace Bon.Serializer.Schemas;

internal class SchemaContentsStoreUpdater(SchemaContentsStore schemaContentsStore, SchemaDataResolver schemaDataResolver)
{
    private readonly Dictionary<(int, SchemaType, bool), CustomSchema> NewCustomSchemas = [];
    private readonly Dictionary<int, SchemaContents> NewSchemaContents = [];

    /// <summary>
    /// Adds all schema contents from the block to the <see cref="SchemaContentsStore"/>.
    /// </summary>
    public void Add(Block block)
    {
        // Create a custom schema for every inner schema that does not yet exist in the store.
        // This is needed because to create a schema contents we need for every member a schema.
        // These custom schemas are created without any members.
        foreach (var schemaContentsData in block.Schemas)
        {
            foreach (var schemaMemberData in schemaContentsData.Members)
            {
                PopulateNewCustomSchemas(schemaMemberData.Schema);
            }
        }

        // Create the new schema contents instances.
        foreach (var schemaContentsData in block.Schemas)
        {
            NewSchemaContents.Add(schemaContentsData.ContentsId, CreateSchemaContents(schemaContentsData.Members));
        }

        // Give every new schema its members.
        foreach (var ((contentsId, _, _), schema) in NewCustomSchemas)
        {
            schema.SetMembers([.. NewSchemaContents[contentsId].Members]);
        }

        // Now that the new schema contents are complete, add them to the store.
        foreach (var (contentsId, schemaContents) in NewSchemaContents)
        {
            AddToStore(contentsId, schemaContents);
        }
    }

    private void PopulateNewCustomSchemas(SchemaData data)
    {
        if (data is CustomSchemaData customSchemaData)
        {
            if (!schemaContentsStore.ContainsContentsId(customSchemaData.ContentsId))
            {
                var key = (customSchemaData.ContentsId, data.SchemaType, data.IsNullable);
                var value = CustomSchema.Create(data.SchemaType, data.IsNullable);
                NewCustomSchemas.TryAdd(key, value);
            }

            return;
        }

        foreach (var innerSchemaData in data.InnerSchemas)
        {
            PopulateNewCustomSchemas(innerSchemaData);
        }
    }

    private SchemaContents CreateSchemaContents(IReadOnlyList<SchemaMemberData> members) => new(
        members
            .Select(member => new SchemaMember(member.Id, GetSchemaBySchemaData(member.Schema)))
            .ToArray());

    /// <summary>
    /// If the input is a <see cref="CustomSchemaData"/> then there are two possibilities:
    /// 1. The class <see cref="SchemaDataResolver"/> can be used to obtain the schema. This means the schema was already
    ///    added to the store (e.g. because it was part of a previous block).
    /// 2. The <see cref="NewCustomSchemas"/> dictionary contains the schema to return. In this case a partial schema can be returned.
    /// 
    /// If the input is not a <see cref="CustomSchemaData"/> then this method is called recursively to obtain the inner schemas.
    /// In this case a new schema instance is created from the inner schemas.
    /// </summary>
    private Schema GetSchemaBySchemaData(SchemaData data)
    {
        if (data is CustomSchemaData customSchemaData)
        {
            var key = (customSchemaData.ContentsId, data.SchemaType, data.IsNullable);

            if (NewCustomSchemas.TryGetValue(key, out var schema))
            {
                return schema;
            }

            return schemaDataResolver.GetSchemaBySchemaData(data);
        }

        var innerSchemas = data.InnerSchemas.Select(GetSchemaBySchemaData).ToArray();

        return Schema.CreateNonCustomSchema(data.SchemaType, data.IsNullable, innerSchemas);
    }

    private void AddToStore(int contentsId, SchemaContents contents)
    {
        if (schemaContentsStore.TryGet(contentsId, out var existingContents))
        {
            if (!SchemaContentsEqualityComparer.Instance.Equals(contents, existingContents))
            {
                throw new InvalidOperationException($"Schema conflict. Found two different schemas with ID {contentsId}.");
            }

            return;
        }

        schemaContentsStore.Add(contentsId, contents);
    }
}
