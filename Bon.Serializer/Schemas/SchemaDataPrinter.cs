namespace Bon.Serializer.Schemas;

internal static class SchemaDataPrinter
{
    public static string Print(SchemaData schemaData)
    {
        if (schemaData is CustomSchemaData customSchemaData)
        {
            return Print(customSchemaData);
        }

        return schemaData.SchemaType switch
        {
            SchemaType.Array => $"{PrintInner(0, 1)}[]",
            SchemaType.Dictionary => "DICTIONARY",
            SchemaType.Tuple2 => $"({PrintInner(0, 2)}, {PrintInner(1, 2)})",
            SchemaType.Tuple3 => $"({PrintInner(0, 3)}, {PrintInner(1, 3)}, {PrintInner(2, 3)})",
            SchemaType.NullableTuple2 => $"{Print(SetSchemaType(schemaData, SchemaType.Tuple2))}?",
            SchemaType.NullableTuple3 => $"{Print(SetSchemaType(schemaData, SchemaType.Tuple3))}?",
            var schemaType => Print(schemaType),
        };

        string PrintInner(int index, int expectedCount)
        {
            var actualCount = schemaData.InnerSchemas.Count;

            if (expectedCount != actualCount)
            {
                throw new DeserializationFailedException(
                    $"Invalid '{schemaData.SchemaType}' schema data. " +
                    $"Found {actualCount} inner schemas but expecting {expectedCount}.");
            }

            return Print(schemaData.InnerSchemas[index]);
        }
    }

    private static string Print(SchemaType schemaType)
    {
        var text = schemaType.ToString();

        return IsCustomNativeType(schemaType) ? text : text.ToLowerInvariant();
    }

    private static bool IsCustomNativeType(SchemaType schemaType)
    {
        return schemaType is SchemaType.WholeNumber or SchemaType.SignedWholeNumber or SchemaType.FractionalNumber;
    }

    private static string Print(CustomSchemaData customSchemaData)
    {
        var schemaType = customSchemaData.SchemaType;

        if (schemaType == SchemaType.NullableRecord)
        {
            return Print(SetSchemaType(customSchemaData, SchemaType.Record)) + "?";
        }

        var prefix = schemaType == SchemaType.Union ? "Union_" : "Record_";

        return prefix + customSchemaData.LayoutId + (schemaType == SchemaType.Union ? "?" : "");
    }

    private static CustomSchemaData SetSchemaType(CustomSchemaData customSchemaData, SchemaType schemaType)
    {
        return new CustomSchemaData(schemaType, customSchemaData.LayoutId);
    }

    private static SchemaData SetSchemaType(SchemaData schemaData, SchemaType schemaType)
    {
        return new SchemaData(schemaType, schemaData.InnerSchemas);
    }

    public static string PrintExplicit(SchemaData schemaData)
    {
        var output = new StringBuilder();

        Add(schemaData, "");

        void Add(SchemaData schemaData, string indentation)
        {
            var extraArgument = schemaData is CustomSchemaData custom ? $", {custom.LayoutId}" : "";
            output.AppendLine($"{indentation}SchemaData(SchemaType.{schemaData.SchemaType}{extraArgument})");

            foreach (var inner in schemaData.InnerSchemas)
            {
                Add(inner, indentation + "    ");
            }
        }

        return output.ToString();
    }
}
