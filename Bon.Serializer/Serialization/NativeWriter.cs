namespace Bon.Serializer.Serialization;

public static class NativeWriter
{
    public const byte NULL = 255;
    public const byte NOT_NULL = 254;

    public static void WriteString(BinaryWriter writer, string? value)
    {
        if (value is null)
        {
            writer.Write(NULL);
        }
        else
        {
            writer.Write(NOT_NULL);
            writer.Write(value);
        }
    }

    public static void WriteBool(BinaryWriter writer, bool? value)
    {
        writer.Write(value switch
        {
            false => (byte)0,
            true => (byte)1,
            null => NULL,
        });
    }

    public static void WriteDecimal(BinaryWriter writer, decimal? value)
    {
        if (value is decimal d)
        {
            writer.Write(NOT_NULL);
            writer.Write(d);
        }
        else
        {
            writer.Write(NULL);
        }
    }

    public static void WriteGuid(BinaryWriter writer, Guid? value) =>
        WriterStore.WriteByteArray(writer, value?.ToByteArray());
}
