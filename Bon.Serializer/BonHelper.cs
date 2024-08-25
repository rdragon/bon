using System.Security.Cryptography;

namespace Bon.Serializer;

internal static class BonHelper
{
    public static uint GetRandomUInt()
    {
        Span<byte> buffer = stackalloc byte[sizeof(uint)];
        RandomNumberGenerator.Fill(buffer);

        return BitConverter.ToUInt32(buffer);
    }

    public static void Ignore<T>(T _)
    {
        // Empty by design.
    }
}
