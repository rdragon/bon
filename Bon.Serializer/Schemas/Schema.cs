namespace Bon.Serializer.Schemas;

public sealed class Schema
{
    public SchemaType SchemaType { get; }

    public IReadOnlyList<Schema> InnerSchemas { get; }

    public int LayoutId { get; set; }

    public IReadOnlyList<SchemaMember> Members { get; set; }

    public bool IsCustomSchema => SchemaType.IsCustomSchema();

    private Schema(
        SchemaType schemaType,
        IReadOnlyList<Schema> innerSchemas,
        int layoutId,
        IReadOnlyList<SchemaMember> members)
    {
        SchemaType = schemaType;
        InnerSchemas = innerSchemas;
        LayoutId = layoutId;
        Members = members;
    }

    public static Schema Create(
        SchemaType schemaType,
        IReadOnlyList<Schema>? innerSchemas = null,
        int layoutId = 0,
        IReadOnlyList<SchemaMember>? members = null)
    {
        return new(schemaType, innerSchemas ?? [], layoutId, members ?? []);
    }
}

public readonly record struct SchemaMember(int Id, Schema Schema);
