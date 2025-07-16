namespace Bon.Serializer.Schemas;

internal static class SchemaSerializer
{
    public static void Write(BinaryWriter writer, SchemaContentsData schema)
    {
        IntSerializer.Write(writer, schema.ContentsId);
        IntSerializer.Write(writer, schema.Members.Count);

        foreach (var member in schema.Members)
        {
            IntSerializer.Write(writer, member.Id);
            Write(writer, member.Schema);
        }
    }

    public static byte[] Write(SchemaData schema)
    {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);
        Write(writer, schema);

        return stream.ToArray();
    }

    public static void Write(BinaryWriter writer, SchemaData schema)
    {
        IntSerializer.Write(writer, (int)schema.SchemaType);

        if (schema is CustomSchemaData custom)
        {
            IntSerializer.Write(writer, custom.ContentsId);

            return;
        }

        foreach (var innerSchema in schema.InnerSchemas)
        {
            Write(writer, innerSchema);
        }
    }

    public static SchemaContentsData ReadSchema(BinaryReader reader)
    {
        var contentsId = IntSerializer.Read(reader) ?? 0;
        var count = IntSerializer.Read(reader) ?? 0;
        var members = new List<SchemaMemberData>(count);

        for (var i = 0; i < count; i++)
        {
            var id = IntSerializer.Read(reader) ?? 0;
            var schemaData = ReadSchemaData(reader);
            members.Add(new SchemaMemberData(id, schemaData));
        }

        return new SchemaContentsData(contentsId, members);
    }

    public static SchemaData ReadSchemaData(byte[] bytes)
    {
        var stream = new MemoryStream(bytes);
        var reader = new BinaryReader(stream);

        return ReadSchemaData(reader);
    }

    public static SchemaData ReadSchemaData(BinaryReader reader)
    {
        var schemaType = (SchemaType)(IntSerializer.Read(reader) ?? 0);

        return schemaType switch
        {
            SchemaType.Record or SchemaType.NullableRecord or SchemaType.Union => new CustomSchemaData(schemaType, IntSerializer.Read(reader) ?? 0),
            SchemaType.Array => new SchemaData(schemaType, [ReadSchemaData(reader)]),
            SchemaType.Dictionary or SchemaType.Tuple2 or SchemaType.NullableTuple2 => new SchemaData(schemaType, [ReadSchemaData(reader), ReadSchemaData(reader)]),
            SchemaType.Tuple3 or SchemaType.NullableTuple3 => new SchemaData(schemaType, [ReadSchemaData(reader), ReadSchemaData(reader), ReadSchemaData(reader)]),
            _ => new SchemaData(schemaType, []),
        };
    }
}
