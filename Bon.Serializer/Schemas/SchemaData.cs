namespace Bon.Serializer.Schemas;

/// <summary>
/// A less recursive version of <see cref="Schema"/> that can be serialized.
/// The recursion stops at <see cref="CustomSchemaData"/>.
/// </summary>
internal record class SchemaData(SchemaType SchemaType, bool IsNullable, IReadOnlyList<SchemaData> InnerSchemas)
{
    public static SchemaData Create(Schema schema)
    {
        return schema switch
        {
            CustomSchema customSchema => new CustomSchemaData(customSchema.SchemaType, customSchema.IsNullable, customSchema.ContentsId),
            _ => new SchemaData(schema.SchemaType, schema.IsNullable, schema.GetInnerSchemas().Select(Create).ToArray()),
        };
    }
}

/// <summary>
/// Represents a record or union.
/// </summary>
internal sealed record class CustomSchemaData(SchemaType SchemaType, bool IsNullable, int ContentsId) :
    SchemaData(SchemaType, IsNullable, []);

/// <summary>
/// Represents a <see cref="CustomSchema"/>.
/// Does not include the schema type which means that the same instance can represent both a record and a union.
/// This is the class that is persisted in the schema storage.
/// </summary>
internal sealed record class SchemaContentsData(int ContentsId, IReadOnlyList<SchemaMemberData> Members);

/// <summary>
/// Represents a member of a record or union.
/// </summary>
/// <param name="Id">The ID from the <see cref="BonMemberAttribute"/>.</param>
/// <param name="Schema">The schema of the member.</param>
internal sealed record class SchemaMemberData(int Id, SchemaData Schema);
