namespace Bon.Serializer;

partial class BonSerializer
{
    /// <summary>
    /// Serializes a value to a byte array.
    /// </summary>
    public byte[] Serialize<T>(T value, BonSerializerOptions? options = null)
    {
        var stream = new MemoryStream();
        Serialize(stream, value, options);

        return stream.ToArray();
    }

    /// <summary>
    /// Serializes a value to a stream.
    /// </summary>
    public void Serialize<T>(Stream stream, T value, BonSerializerOptions? options = null)
    {
        var writer = new BinaryWriter(stream);

        var (writeValue, usesCustomSchemas, simpleWriterType) = _writerStore.GetWriter<T>();

        if (options?.AllowSchemaTypeOptimization != false && simpleWriterType != SimpleWriterType.None)
        {
            var output = new BonOutput(writer, options);
            _simpleWriterStore.GetWriter<T>(simpleWriterType)(output, value);
            return;
        }

        if (options?.IncludeHeader != false)
        {
            WriteHeader(writer, typeof(T));
        }

        writeValue(writer, value);
    }

    private void WriteHeader(BinaryWriter writer, Type type)
    {
        var schemaData = _schemaDataStore.GetSchemaData(type);
        WriteHeader(writer, schemaData);
    }

    private static void WriteHeader(BinaryWriter writer, SchemaData schemaData)
    {
        SchemaSerializer.Write(writer, schemaData);
    }
}
