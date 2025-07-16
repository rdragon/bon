namespace Bon.Serializer.Serialization;

/// <summary>
/// Serializes whole numbers as variable-width integers.
/// This class is an alternative to using the method <see cref="BinaryWriter.Write7BitEncodedInt64(long)"/>.
/// 
/// In general the output of this class is a bit less compact as the steps are more coarse. Instead of allowing the output to be of any
/// length between 1 and 9 bytes only the following lengths are possible: 1, 2, 3, 5, 9.
/// 
/// However, in general this class is faster than the Write7BitEncodedInt64 and Read7BitEncodedInt64 methods. This is because
/// the first byte determines the amount of bytes to be read. The Read7BitEncodedInt64 needs to read one byte at a time to determine
/// if the end of the number has been reached.
/// 
/// Also, this class can serialize and deserialize the value null (null is represented by the byte 255).
/// 
/// The <see cref="WriteSigned"/> method can be used to serialize negative numbers in a compact way.
/// </summary>
public static class WholeNumberSerializer
{
    /// <summary>
    /// Writes the value to the stream using 1, 2, 3, 5, or 9 bytes.
    /// </summary>
    public static void Write(BinaryWriter writer, ulong? value)
    {
        if (value is not { } number)
        {
            writer.Write(NativeWriter.NULL);
            return;
        }

        switch (number)
        {
            case < (1 << 7):
                writer.Write((byte)number);
                break;

            case < (1 << 14):
                writer.Write((byte)(128 | (number >> 8)));
                writer.Write((byte)number);
                break;

            case < (1 << 21):
                writer.Write((byte)(128 | 64 | (number >> 16)));
                writer.Write((ushort)number);
                break;

            case < (1L << 36):
                writer.Write((byte)(128 | 64 | 32 | (number >> 32)));
                writer.Write((uint)number);
                break;

            default:
                writer.Write((byte)(128 | 64 | 32 | 16));
                writer.Write(number);
                break;
        }
    }

    /// <summary>
    /// Writes the value to the stream using 1, 2, 3, 5, or 9 bytes.
    /// The ZigZag encoding is used so that negative values take up around the same amount of bytes as positive values.
    /// </summary>
    public static void WriteSigned(BinaryWriter writer, long? value) => Write(writer, ConvertToUnsigned(value));

    public static ulong? Read(BinaryReader reader)
    {
        var b = reader.ReadByte();

        return b switch
        {
            NativeWriter.NULL => null,
            < 128 => b,
            < 192 => ((ulong)(b & 127) << 8) | reader.ReadByte(),
            < 224 => ((ulong)(b & 63) << 16) | reader.ReadUInt16(),
            < 240 => ((ulong)(b & 31) << 32) | reader.ReadUInt32(),
            _ => reader.ReadUInt64(),
        };
    }

    public static long? ReadSigned(this BinaryReader reader) => ConvertToSigned(Read(reader));

    // The ZigZag encoding, see https://protobuf.dev/programming-guides/encoding/.
    private static ulong ConvertToUnsigned(long value) => (ulong)((value << 1) ^ (value >> 63));

    private static ulong? ConvertToUnsigned(long? value) => value is long l ? ConvertToUnsigned(l) : null;

    private static long ConvertToSigned(ulong value) => (long)(value >> 1) ^ ((long)(value << 63) >> 63);

    private static long? ConvertToSigned(ulong? value) => value is ulong u ? ConvertToSigned(u) : null;
}
