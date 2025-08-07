namespace Bon.Serializer;

partial class BonSerializer
{
    public string PrintKnownLayouts()
    {
        var builder = new StringBuilder();
        builder.AppendLine("// -------------");
        builder.AppendLine("// KNOWN LAYOUTS");
        builder.AppendLine("// -------------");
        builder.AppendLine();
        builder.AppendLine(new FullSchemaPrinter().Print(_layoutStore.Layouts));
        return builder.ToString();
    }

    public string PrintKnownSchemas(Predicate<Schema>? filter = null)
    {
        var schemas = _schemaStore.Schemas.Where(pair => filter is null || filter(pair.Value));
        var builder = new StringBuilder();
        builder.AppendLine("// -------------");
        builder.AppendLine("// KNOWN SCHEMAS");
        builder.AppendLine("// -------------");
        builder.AppendLine();
        builder.AppendLine(new FullSchemaPrinter().Print(schemas));
        return builder.ToString();
    }

    public static string PrintLayouts(byte[] blob)
    {
        var store = new LayoutStore();
        var reader = new LayoutReader(store, new BinaryReader(new MemoryStream(blob)), true);
        reader.ReadManyLayouts();
        return new FullSchemaPrinter().Print(store.Layouts);
    }

    /// <summary>
    /// Prints the schema of the provided message.
    /// </summary>
    /// <param name="message">The message to print the schema of.</param>
    /// <param name="printFullSchema">
    /// Whether to also print all inner schemas found in the schema.
    /// If false then the printing stops at custom schemas.
    /// Instead of their members, only their layout IDs are printed.
    /// If true then the schemas are printed in C# syntax (with class names including the schema type and layout ID).
    /// </param>
    public string PrintSchema(byte[] message, bool printFullSchema = true)
    {
        var reader = new BinaryReader(new MemoryStream(message));
        var schema = ReadSchema(reader);
        return printFullSchema ? new FullSchemaPrinter().Print(schema) : LimitedSchemaPrinter.PrintSingleLine(schema);
    }

    /// <summary>
    /// Prints the schema corresponding to the given type.
    /// </summary>
    /// <param name="printFullSchema">
    /// Whether to also print all inner schemas found in the schema.
    /// If false then the printing stops at custom schemas.
    /// Instead of their members, only their layout IDs are printed.
    /// If true then the schemas are printed in C# syntax (with class names including the schema type and layout ID).
    /// </param>
    public string PrintSchema<T>(bool printFullSchema = true) => PrintSchema(Serialize<T?>(default), printFullSchema);

    /// <summary>
    /// Prints the header of the provided message.
    /// </summary>
    /// <param name="message">The message to print the header of.</param>
    /// <param name="multiLineFormatting">Whether to print the schema in a multi-line format.</param>
    public string PrintHeader(byte[] message, bool multiLineFormatting = false)
    {
        var reader = new BinaryReader(new MemoryStream(message));
        var schema = ReadSchema(reader);
        var schemaBytes = message[..(int)reader.BaseStream.Position];
        var builder = new StringBuilder();
        builder.AppendLine("Header");
        builder.AppendLine("Length       " + schemaBytes.Length);
        builder.AppendLine("Hexadecimal  " + schemaBytes.ToHexString());

        if (multiLineFormatting)
        {
            builder.AppendLine("Formatted");
            builder.AppendLine("{");
            builder.Append(LimitedSchemaPrinter.PrintMultiLine(schema, "    "));
            builder.AppendLine("}");
        }
        else
        {
            builder.AppendLine("Formatted    " + LimitedSchemaPrinter.PrintSingleLine(schema));
        }

        return builder.ToString();
    }

    /// <summary>
    /// Prints the body of the provided message.
    /// </summary>
    public string PrintBody(byte[] message)
    {
        var reader = new BinaryReader(new MemoryStream(message));
        ReadSchema(reader);
        var body = message[(int)reader.BaseStream.Position..];
        var builder = new StringBuilder();
        builder.AppendLine("Body");
        builder.AppendLine("Length       " + body.Length);
        builder.AppendLine("Hexadecimal  " + body.ToHexString());
        return builder.ToString();
    }
}
