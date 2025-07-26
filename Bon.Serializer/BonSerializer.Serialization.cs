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

        var (writeValue, simpleWriterType) = _writerStore.GetWriter<T>();

        if (options?.AllowSchemaTypeOptimization != false && simpleWriterType != SimpleWriterType.None)
        {
            var output = new BonOutput(writer, options);
            SimpleWriter.GetWriter<T>(simpleWriterType)(output, value);
            return;
        }

        if (options?.IncludeHeader != false)
        {
            WriteSchema(writer, typeof(T));
        }

        writeValue(writer, value);
    }

    private void WriteSchema(BinaryWriter writer, Type type)
    {
        WriteSchema(writer, _schemaStore.GetOrAddSchema(type));
    }

    private static void WriteSchema(BinaryWriter writer, Schema schema)
    {
        new LayoutWriter(writer).WriteSchema(schema);
    }
}
