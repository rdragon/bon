namespace Bon.Serializer.Schemas;

/// <summary>
/// Prints schemas but stops at custom schemas.
/// Prints schema types, schema arguments and layout IDs.
/// Instead of the members of a custom schema, only its layout ID is printed.
/// </summary>
internal static class LimitedSchemaPrinter
{
    /// <summary>
    /// Prints the schema using a single line format.
    /// Prints schema types, schema arguments and layout IDs.
    /// Instead of the members of a custom schema, only its layout ID is printed.
    /// </summary>
    public static string PrintSingleLine(Schema schema)
    {
        if (schema.IsCustom)
        {
            return PrintCustomSchema(schema);
        }

        return schema.SchemaType switch
        {
            SchemaType.Array => $"{PrintSchemaArgument(0, 1)}[]",
            SchemaType.Dictionary => "DICTIONARY",
            SchemaType.Tuple2 => $"({PrintSchemaArgument(0, 2)}, {PrintSchemaArgument(1, 2)})",
            SchemaType.Tuple3 => $"({PrintSchemaArgument(0, 3)}, {PrintSchemaArgument(1, 3)}, {PrintSchemaArgument(2, 3)})",
            SchemaType.NullableTuple2 => $"{PrintSingleLine(schema.GetClone(SchemaType.Tuple2))}?",
            SchemaType.NullableTuple3 => $"{PrintSingleLine(schema.GetClone(SchemaType.Tuple3))}?",
            var schemaType => PrintSchemaType(schemaType),
        };

        string PrintSchemaArgument(int index, int expectedCount)
        {
            var actualCount = schema.SchemaArguments.Count;

            if (expectedCount != actualCount)
            {
                throw new DeserializationFailedException(
                    $"Invalid '{schema.SchemaType}' schema. " +
                    $"Found {actualCount} schema argument(s) but expecting {expectedCount}.");
            }

            return PrintSingleLine(schema.SchemaArguments[index]);
        }
    }

    /// <summary>
    /// Prints the schema using a multi-line format.
    /// Prints schema types, schema arguments and layout IDs.
    /// Instead of the members of a custom schema, only its layout ID is printed.
    /// </summary>
    public static string PrintMultiLine(Schema schema, string indentation = "")
    {
        var output = new StringBuilder();

        Add(schema, indentation);

        void Add(Schema schema, string indentation)
        {
            var suffix = schema.IsCustom ? $"_{schema.LayoutId}" : "";
            output.AppendLine($"{indentation}{schema.SchemaType}{suffix}");

            foreach (var schemaArgument in schema.SchemaArguments)
            {
                Add(schemaArgument, indentation + "    ");
            }
        }

        return output.ToString();
    }

    public static string PrintSchemaType(SchemaType schemaType)
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
}
