namespace Bon.Serializer.Schemas;

internal static class SchemaSerializer
{
    public static void Write(BinaryWriter writer, SchemaContentsData schema)
    {
        WholeNumberSerializer.Write(writer, schema.ContentsId);
        WholeNumberSerializer.Write(writer, schema.Members.Count);

        foreach (var member in schema.Members)
        {
            WholeNumberSerializer.Write(writer, member.Id);
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
        WholeNumberSerializer.Write(writer, (int)schema.SchemaType);
        writer.Write(schema.IsNullable);

        if (schema is CustomSchemaData custom)
        {
            WholeNumberSerializer.Write(writer, custom.ContentsId);

            return;
        }

        foreach (var innerSchema in schema.InnerSchemas)
        {
            Write(writer, innerSchema);
        }
    }

    public static SchemaContentsData ReadSchema(BinaryReader reader)
    {
        var contentsId = (int)WholeNumberSerializer.Read(reader);
        var count = (int)WholeNumberSerializer.Read(reader);
        var members = new List<SchemaMemberData>(count);

        for (var i = 0; i < count; i++)
        {
            var id = (int)WholeNumberSerializer.Read(reader);
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
        var schemaType = (SchemaType)WholeNumberSerializer.Read(reader);
        var isNullable = reader.ReadBoolean();

        return schemaType switch
        {
            SchemaType.Record or SchemaType.Union => new CustomSchemaData(schemaType, isNullable, (int)WholeNumberSerializer.Read(reader)),
            SchemaType.Array => new SchemaData(schemaType, isNullable, [ReadSchemaData(reader)]),
            SchemaType.Dictionary or SchemaType.Tuple2 => new SchemaData(schemaType, isNullable, [ReadSchemaData(reader), ReadSchemaData(reader)]),
            SchemaType.Tuple3 => new SchemaData(schemaType, isNullable, [ReadSchemaData(reader), ReadSchemaData(reader), ReadSchemaData(reader)]),
            _ => new SchemaData(schemaType, isNullable, []),
        };
    }
}
