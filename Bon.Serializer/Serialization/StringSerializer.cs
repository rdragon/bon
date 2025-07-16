using System.Buffers;

namespace Bon.Serializer.Serialization;

public static class StringSerializer
{
    private const int MaxArrayPoolRentalSize = 64 * 1024;

    public static void WriteString(BinaryWriter writer, string? value)
    {
        if (value is null)
        {
            IntSerializer.WriteNull(writer);

            return;
        }

        if (value.Length <= 127 / 3)
        {
            Span<byte> buffer = stackalloc byte[128];
            var actualByteCount = Encoding.UTF8.GetBytes(value, buffer[1..]);
            buffer[0] = (byte)actualByteCount;
            writer.Write(buffer[..(actualByteCount + 1)]);

            return;
        }

        var rentSize = value.Length <= MaxArrayPoolRentalSize / 3 ? value.Length * 3 : Encoding.UTF8.GetByteCount(value);

        if (rentSize <= MaxArrayPoolRentalSize)
        {
            var rented = ArrayPool<byte>.Shared.Rent(rentSize);
            var actualByteCount = Encoding.UTF8.GetBytes(value, rented);
            IntSerializer.Write(writer, actualByteCount);
            writer.Write(rented, 0, actualByteCount);
            ArrayPool<byte>.Shared.Return(rented);

            return;
        }

        WriteLargeString(writer, value, rentSize);
    }

    private static void WriteLargeString(BinaryWriter writer, ReadOnlySpan<char> chars, int actualByteCount)
    {
        IntSerializer.Write(writer, actualByteCount);

        var rented = ArrayPool<byte>.Shared.Rent(MaxArrayPoolRentalSize);
        var encoder = Encoding.UTF8.GetEncoder();
        bool completed;

        do
        {
            encoder.Convert(chars, rented, flush: true, out int charsConsumed, out int bytesWritten, out completed);

            if (bytesWritten != 0)
            {
                writer.Write(rented, 0, bytesWritten);
            }

            chars = chars[charsConsumed..];
        } while (!completed);

        ArrayPool<byte>.Shared.Return(rented);
    }
}
