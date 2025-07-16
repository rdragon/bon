using System.Runtime.CompilerServices;

namespace Bon.Serializer.Serialization;

public static class StringDeserializer
{
    private const int BufferSize = 128;

    public static string? ReadString(BinaryReader reader)
    {
        if (IntSerializer.Read(reader) is not int totalByteCount)
        {
            return null;
        }

        return ReadString(reader, totalByteCount);
    }

    private static string ReadString(BinaryReader reader, int totalByteCount)
    {
        if (totalByteCount == 0)
        {
            return "";
        }

        var decoder = GetDecoder(reader);
        Span<byte> bytesBuffer = stackalloc byte[BufferSize];
        Span<char> charsBuffer = stackalloc char[BufferSize];
        var currentIndex = 0;

        StringBuilder? builder = null;

        do
        {
            var bytes = bytesBuffer[..Math.Min(totalByteCount - currentIndex, BufferSize)];
            var byteReadCount = reader.Read(bytes);

            if (byteReadCount == 0)
            {
                throw new DeserializationFailedException("Unexpected end of stream while reading string.");
            }

            var charReadCount = decoder.GetChars(bytes, charsBuffer, false);
            var chars = charsBuffer[..charReadCount];

            if (currentIndex == 0 && byteReadCount == totalByteCount)
            {
                return new string(chars);
            }

            builder ??= StringBuilderPool.Shared.Rent();
            builder.Append(chars);
            currentIndex += byteReadCount;
        } while (currentIndex < totalByteCount);

        var result = builder.ToString();
        StringBuilderPool.Shared.Return(builder);

        return result;
    }

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_decoder")]
    extern static ref Decoder GetDecoder(BinaryReader reader);
}
