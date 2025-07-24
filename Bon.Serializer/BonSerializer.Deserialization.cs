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

    private T DeserializeBody<T>(BinaryReader reader, Schema1 schema)
    {
        var deserialize = _deserializerStore.GetDeserializer<T>(schema);
        var input = new BonInput(reader);
        return deserialize(input);
    }

    /// <summary>
    /// //2at
    /// </summary>
    public string PrintSchema(byte[] message)
    {
        var reader = new BinaryReader(new MemoryStream(message));
        var schema = ReadSchema(reader);
        return new SchemaPrinter().Print(schema);
    }

    /// <summary>
    /// //2at
    /// </summary>
    public string PrintSchema<T>() => PrintSchema(Serialize<T?>(default));

    private Schema1 ReadSchema(BinaryReader reader)
    {
        var data = SchemaSerializer.ReadSchemaData(reader);
        return _schemaDataResolver.GetSchemaBySchemaData(data);
    }
}
