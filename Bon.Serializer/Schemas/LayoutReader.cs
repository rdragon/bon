using System.Diagnostics;

namespace Bon.Serializer.Schemas;

internal sealed class LayoutReader(LayoutStore layoutStore, BinaryReader reader)
{
    public void Read()
    {
        var id = ReadInt(min: 1);
        var members = ReadMembers();
        var layout = new Layout(id, members);
        layoutStore.Add(layout);
    }

    private IReadOnlyList<SchemaMember> ReadMembers()
    {
        var count = ReadInt(0, 10_000);
        var members = new SchemaMember[count];

        for (int i = 0; i < count; i++)
        {
            members[i] = ReadMember();
        }

        return members;
    }

    private SchemaMember ReadMember()
    {
        var id = ReadInt(min: 0);
        var schema = ReadSchema();
        return new SchemaMember(id, schema);
    }

    private Schema ReadSchema()
    {
        var schemaType = ReadSchemaType();

        if (schemaType.IsNativeSchema())
        {
            return NativeSchema.FromSchemaType(schemaType);
        }

        var innerSchemas = ReadInnerSchemas(schemaType);
        var layoutId = ReadLayoutId(schemaType);//3at: hier moeten we dict lookup doen van schematype + layoutid als het custom is. als we ref eq willen doen
        var members = ReadMembers(schemaType, layoutId);
        var schema = new Schema(schemaType, innerSchemas, layoutId, members!);

        if (members is null)
        {
            layoutStore.PartialSchemas.Add(schema);
        }

        return schema;
    }

    private IReadOnlyList<Schema> ReadInnerSchemas(SchemaType schemaType)
    {
        var count = GetInnerSchemaCount(schemaType);
        var innerSchemas = new Schema[count];

        for (int i = 0; i < count; i++)
        {
            innerSchemas[i] = ReadSchema();
        }

        return innerSchemas;
    }

    private int ReadLayoutId(SchemaType schemaType) => schemaType.IsCustomSchema() ? ReadInt(min: 1) : 0;

    private IReadOnlyList<SchemaMember>? ReadMembers(SchemaType schemaType, int layoutId)
    {
        if (!schemaType.IsCustomSchema())
        {
            return [];
        }

        if (layoutStore.TryGet(layoutId, out var layout))
        {
            return layout.Members;
        }

        return null;
    }

    private int ReadInt(int min = int.MinValue, int max = int.MaxValue)
    {
        var value = IntSerializer.Read(reader) ?? 0;
        Trace.Assert(min <= value && value <= max);
        return value;
    }

    private SchemaType ReadSchemaType() => (SchemaType)reader.ReadByte();

    private static int GetInnerSchemaCount(SchemaType schemaType)
    {
        return schemaType switch
        {
            SchemaType.Array => 1,
            SchemaType.Tuple2 or SchemaType.NullableTuple2 or SchemaType.Dictionary => 2,
            SchemaType.Tuple3 or SchemaType.NullableTuple3 => 3,
            _ => 0,
        };
    }
}
