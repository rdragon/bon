namespace Bon.Serializer;

partial class BonSerializer
{
    /// <summary>
    /// Deserializes a value from a byte array.
    /// </summary>
    public T? Deserialize<T>(byte[] bytes)
    {
        var stream = new MemoryStream(bytes);
        var result = Deserialize<T>(stream);
        if (stream.Position < stream.Length)
        {
            throw new DeserializationFailedException($"{stream.Length - stream.Position} bytes were not read.");
        }
        return result;
    }

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

    private Schema ReadSchema(BinaryReader reader)
    {
        return new LayoutReader(_layoutStore, reader, false).ReadSingleSchema();
    }
}
