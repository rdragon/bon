namespace Bon.Serializer.Schemas;

/// <summary>
/// Represents a native, array or tuple schema.
/// Records and unions are represented by <see cref="CustomSchemaData"/>.
/// A less recursive version of <see cref="Schema1"/> that can be serialized.
/// The recursion stops at <see cref="CustomSchemaData"/>.
/// </summary>
internal record class SchemaData(SchemaType SchemaType, IReadOnlyList<SchemaData> InnerSchemas)
{
    public static SchemaData Create(Schema1 schema)
    {
        return schema switch
        {
            CustomSchema customSchema => new CustomSchemaData(customSchema.SchemaType, customSchema.LayoutId),
            _ => new SchemaData(schema.SchemaType, schema.GetInnerSchemas().Select(Create).ToArray()),
        };
    }
}

/// <summary>
/// Represents a record or union.
/// </summary>
internal sealed record class CustomSchemaData(SchemaType SchemaType, int LayoutId) :
    SchemaData(SchemaType, []);

/// <summary>
/// Represents a <see cref="CustomSchema"/>.
/// Does not include the schema type which means that the same instance can represent both a record and a union.
/// This is the class that is persisted in the schema storage.
/// </summary>
internal sealed record class SchemaContentsData(int LayoutId, IReadOnlyList<SchemaMemberData> Members);

/// <summary>
/// Represents a member of a record or union.
/// </summary>
/// <param name="Id">The ID from the <see cref="BonMemberAttribute"/>.</param>
/// <param name="Schema">The schema of the member.</param>
internal sealed record class SchemaMemberData(int Id, SchemaData Schema);
