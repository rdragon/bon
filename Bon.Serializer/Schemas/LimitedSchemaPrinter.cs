namespace Bon.Serializer.Schemas;

//2at, stopt bij custom schemas, print geen members, alleen inner schemas en schema types en layout ids
internal static class LimitedSchemaPrinter
{
    public static string PrintSingleLine(Schema schema)
    {
        if (schema.IsCustom)
        {
            return PrintCustomSchema(schema);
        }

        return schema.SchemaType switch
        {
            SchemaType.Array => $"{PrintInner(0, 1)}[]",
            SchemaType.Dictionary => "DICTIONARY",
            SchemaType.Tuple2 => $"({PrintInner(0, 2)}, {PrintInner(1, 2)})",
            SchemaType.Tuple3 => $"({PrintInner(0, 3)}, {PrintInner(1, 3)}, {PrintInner(2, 3)})",
            SchemaType.NullableTuple2 => $"{PrintSingleLine(schema.GetClone(SchemaType.Tuple2))}?",
            SchemaType.NullableTuple3 => $"{PrintSingleLine(schema.GetClone(SchemaType.Tuple3))}?",
            var schemaType => Print(schemaType),
        };

        string PrintInner(int index, int expectedCount)
        {
            var actualCount = schema.InnerSchemas.Count;

            if (expectedCount != actualCount)
            {
                throw new DeserializationFailedException(
                    $"Invalid '{schema.SchemaType}' schema data. " +
                    $"Found {actualCount} inner schemas but expecting {expectedCount}.");
            }

            return PrintSingleLine(schema.InnerSchemas[index]);
        }
    }

    public static string Print(SchemaType schemaType)
    {
        var text = schemaType == SchemaType.NullableDecimal ? "decimal" : schemaType.ToString();
        var suffix = schemaType.IsNullable() ? "?" : "";

        return IsSpecialNativeType(schemaType) ? text + suffix : text.ToLowerInvariant() + suffix;
    }

    private static bool IsSpecialNativeType(SchemaType schemaType)
    {
        return schemaType is SchemaType.WholeNumber or SchemaType.SignedWholeNumber or SchemaType.FractionalNumber;
    }

    private static string PrintCustomSchema(Schema schema)
    {
        var schemaType = schema.SchemaType;

        if (schemaType == SchemaType.NullableRecord)
        {
            return PrintSingleLine(schema.GetClone(SchemaType.Record)) + "?";
        }

        var prefix = schemaType == SchemaType.Union ? "Union_" : "Record_";

        return prefix + schema.LayoutId + (schemaType == SchemaType.Union ? "?" : "");
    }

    //2at
    public static string PrintMultiLine(Schema schema, string indentation = "")
    {
        var output = new StringBuilder();

        Add(schema, indentation);

        void Add(Schema schema, string indentation)
        {
            var suffix = schema.IsCustom ? $"_{schema.LayoutId}" : "";
            output.AppendLine($"{indentation}{schema.SchemaType}{suffix}");

            foreach (var inner in schema.InnerSchemas)
            {
                Add(inner, indentation + "    ");
            }
        }

        return output.ToString();
    }
}
