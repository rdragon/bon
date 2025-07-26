namespace Bon.Serializer;

partial class BonSerializer
{
    /// <summary>
    /// Deserializes a value from a byte array.
    /// </summary>
    public T? Deserialize<T>(byte[] bytes) => Deserialize<T>(new MemoryStream(bytes));

    /// <summary>
    /// Deserializes a value from a stream.
    /// </summary>
    public T? Deserialize<T>(Stream stream)
    {
        var reader = new BinaryReader(stream);
        var schema = ReadSchema(reader);
        return DeserializeBody<T>(reader, schema);
    }

    private T DeserializeBody<T>(BinaryReader reader, Schema schema)
    {
        var deserialize = _deserializerStore.GetDeserializer<T>(schema);
        var input = new BonInput(reader);
        return deserialize(input);
    }

    /// <summary>
    /// //2at
    /// </summary>
    public string PrintSchema(byte[] message, bool printFullSchema = true)
    {
        var reader = new BinaryReader(new MemoryStream(message));
        var schema = ReadSchema(reader);
        return printFullSchema ? new FullSchemaPrinter().Print(schema) : LimitedSchemaPrinter.PrintSingleLine(schema);
    }

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
    /// //2at
    /// </summary>
    public string PrintSchema<T>() => PrintSchema(Serialize<T?>(default));

    private Schema ReadSchema(BinaryReader reader)
    {
        return new LayoutReader(_layoutStore, reader, false).ReadSingleSchema();
    }
}
