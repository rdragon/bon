namespace Bon.Serializer.Serialization;

public static class NativeReader
{
    public static bool? ReadBool(BinaryReader reader)
    {
        return reader.ReadByte() switch
        {
            0 => false,
            1 => true,
            _ => null,
        };
    }

    public static decimal? ReadDecimal(BinaryReader reader) => reader.ReadByte() == NativeWriter.NULL ? null : reader.ReadDecimal();

    public static Guid? ReadGuid(BinaryReader reader) => CollectionDeserializer.ReadByteArray(reader)?.ToGuid();
}
