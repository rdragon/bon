namespace Bon.Serializer.Schemas;

internal sealed class LayoutReader(LayoutStore layoutStore, BinaryReader reader, bool allowUnknownLayoutIds)
{
    //2at
    private List<Schema>? _partialSchemas = null;

    public void ReadManyLayouts()
    {
        _partialSchemas = [];
        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            ReadSingleLayout();
        }
    }

    private void ReadSingleLayout()
    {
        WholeNumberSerializer.Read(reader);
        var id = ReadLayoutId();
        var members = ReadMembers();
        var layout = new Layout(id, members);
        layoutStore.AddLayout(layout);
        FillPartialSchemas(layout);
    }

    public Schema ReadSingleSchema()
    {
        var schemaType = ReadSchemaType();

        if (schemaType.IsNativeSchema())
        {
            return Schema.GetNativeSchema(schemaType);
        }

        var innerSchemas = ReadInnerSchemas(schemaType);
        var layoutId = ReadLayoutId(schemaType);
        var members = ReadMembers(schemaType, layoutId);
        var schema = Schema.Create(schemaType, innerSchemas, layoutId, members!);

        if (members is null)
        {
            _partialSchemas!.Add(schema);
        }

        return schema;
    }

    private IReadOnlyList<SchemaMember> ReadMembers()
    {
        var count = ReadInt("Member count out of range", 0, 10_000);
        var members = new SchemaMember[count];

        for (int i = 0; i < count; i++)
        {
            members[i] = ReadMember();
        }

        return members;
    }

    private SchemaMember ReadMember()
    {
        var id = ReadInt("Member ID out of range", min: 0);
        var schema = ReadSingleSchema();
        return new SchemaMember(id, schema);
    }

    private IReadOnlyList<Schema> ReadInnerSchemas(SchemaType schemaType)
    {
        var count = GetInnerSchemaCount(schemaType);
        var innerSchemas = new Schema[count];

        for (int i = 0; i < count; i++)
        {
            innerSchemas[i] = ReadSingleSchema();
        }

        return innerSchemas;
    }

    private int ReadLayoutId(SchemaType schemaType) => schemaType.IsCustomSchema() ? ReadLayoutId() : 0;

    private IReadOnlyList<SchemaMember>? ReadMembers(SchemaType schemaType, int layoutId)
    {
        if (!schemaType.IsCustomSchema())
        {
            return [];
        }

        if (!allowUnknownLayoutIds)
        {
            return layoutStore.GetLayout(layoutId).Members;
        }

        return layoutStore.TryGetLayout(layoutId, out var layout) ? layout.Members : null;
    }

    private void FillPartialSchemas(Layout layout)
    {
        for (int i = 0; i < _partialSchemas!.Count; i++)
        {
            if (_partialSchemas[i].LayoutId == layout.Id)
            {
                _partialSchemas[i].Members = layout.Members;
                _partialSchemas[i] = _partialSchemas[^1];
                _partialSchemas.RemoveAt(_partialSchemas.Count - 1);
                i--;
            }
        }
    }

    private int ReadLayoutId() => ReadInt("Layout ID out of range", min: 1);

    private int ReadInt(string? message, int min = int.MinValue, int max = int.MaxValue)
    {
        var value = IntSerializer.Read(reader) ?? 0;
        Trace.Assert(min <= value && value <= max, message);
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
