namespace Bon.Serializer.Serialization;

// This class is directly used by the source generated code.
public static class IntSerializer
{
    /// <summary>
    /// Writes a value to the stream using 1, 2, 3, or 5 bytes.
    /// Negative values are allowed, but always take up 5 bytes.
    /// Use <see cref="WholeNumberSerializer.WriteSigned"/> if you expect negative values.
    /// </summary>
    public static void Write(BinaryWriter writer, int? value) => WholeNumberSerializer.Write(writer, (uint?)value);

    public static int? Read(BinaryReader reader) => (int?)WholeNumberSerializer.Read(reader);

    public static void WriteNull(BinaryWriter writer) => writer.Write(NativeWriter.NULL);
}
