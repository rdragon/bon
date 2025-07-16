namespace Bon.Serializer;

partial class BonSerializer
{
    /// <summary>
    /// Deserializes a value from a byte array.
    /// Loads new schemas from the storage if unknown schema IDs are encountered.
    /// </summary>
    public Task<T?> DeserializeAsync<T>(byte[] bytes) => DeserializeAsync<T>(new MemoryStream(bytes));

    /// <summary>
    /// Deserializes a value from a stream.
    /// Loads new schemas from the storage if unknown schema IDs are encountered.
    /// </summary>
    public async Task<T?> DeserializeAsync<T>(Stream stream)
    {
        var reader = new BinaryReader(stream);
        var header = await ReadHeaderAsync(reader).ConfigureAwait(false);

        return DeserializeBody<T>(reader, header.SchemaData);
    }

    /// <summary>
    /// Deserializes a value from a byte array.
    /// If unknown schema IDs are encountered then an exception is thrown.
    /// </summary>
    public T? Deserialize<T>(byte[] bytes) => Deserialize<T>(new MemoryStream(bytes));

    /// <summary>
    /// Deserializes a value from a stream.
    /// If unknown schema IDs are encountered then an exception is thrown.
    /// </summary>
    public T? Deserialize<T>(Stream stream)
    {
        var reader = new BinaryReader(stream);
        var header = ReadHeader(reader);
        return DeserializeBody<T>(reader, header.SchemaData);
    }

    /// <summary>
    /// Deserializes a value from a stream.
    /// If unknown schema IDs are encountered then false is returned and the out parameter is set to the default value.
    /// </summary>
    public bool TryDeserialize<T>(Stream stream, out T? value)
    {
        var reader = new BinaryReader(stream);

        if (TryReadHeader(reader, out var header))
        {
            value = DeserializeBody<T>(reader, header.SchemaData);
            return true;
        }

        value = default;
        return false;
    }

    private T DeserializeBody<T>(BinaryReader reader, SchemaData schemaData)
    {
        var schema = _schemaDataResolver.GetSchemaBySchemaData(schemaData);
        var deserialize = _deserializerStore.GetDeserializer<T>(schema);
        var input = new BonInput(reader);

        return deserialize(input);
    }
}
