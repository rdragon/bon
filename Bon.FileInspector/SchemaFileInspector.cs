namespace Bon.FileInspector;

internal static class SchemaFileInspector
{
    public static void Run(Stream inputStream, Stream outputStream)
    {
        var writer = new StreamWriter(outputStream);
        writer.WriteLine("schema file");
        writer.WriteLine();

        foreach (var block in BlockSerializer.Deserialize(inputStream))
        {
            writer.WriteLine($"block {block.BlockId}");

            foreach (var schema in block.Schemas)
            {
                ConvertSchemaData(schema, writer);
            }
        }

        writer.Flush();
    }

    private static void ConvertSchemaData(SchemaContentsData schema, StreamWriter writer)
    {
        writer.WriteLine($"    schema {schema.ContentsId}");

        foreach (var member in schema.Members)
        {
            writer.WriteLine($"        {member.Id}: {ConvertSchemaData(member.Schema)}");
        }

        if (schema.Members.Count == 0)
        {
            writer.WriteLine("        (empty)");
        }

        writer.WriteLine();
    }

    private static string ConvertSchemaData(SchemaData schema)
    {
        if (schema is CustomSchemaData customSchema)
        {
            return $"{schema.SchemaType}_{customSchema.ContentsId}";
        }

        var text = string.Join(", ", schema.InnerSchemas.Select(ConvertSchemaData));

        return schema.SchemaType switch
        {
            SchemaType.Array => $"{text}[]",
            SchemaType.Dictionary => $"Dictionary<{text}>",
            SchemaType.Tuple2 or SchemaType.Tuple3 => $"({text})",
            SchemaType.Tuple2Maybe or SchemaType.Tuple3Maybe => $"({text})?",
            _ => schema.SchemaType.ToString(),
        };
    }
}
