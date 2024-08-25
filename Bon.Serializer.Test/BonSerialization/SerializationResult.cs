using System.Collections;
using System.Text.Json.Nodes;

namespace Bon.Serializer.Test.BonSerialization;

public sealed class SerializationResult(BonSerializer bonSerializer, MemoryStream stream) : IEnumerable<byte>
{
    public byte[] Bytes => stream.ToArray();

    public T DeserializeFast<T>() => Deserialize<T>(false);

    public T DeserializeSlow<T>() => Deserialize<T>(true);

    public T Deserialize<T>(bool expectNewDeserializer)
    {
        stream.Position = 0;
        var countBefore = bonSerializer.DeserializerCount;
        var result = bonSerializer.DeserializeAsync<T>(stream).Result;
        Assert.Equal(stream.Length, stream.Position);

        var actual = bonSerializer.DeserializerCount != countBefore ? "slow" : "fast";
        Assert.Equal(expectNewDeserializer ? "slow" : "fast", actual);

        return result;
    }

    public JsonObject DeserializeToJson()
    {
        stream.Position = 0;

        return bonSerializer.BonToJsonAsync(stream).Result;
    }

    public IEnumerator<byte> GetEnumerator() => ((IEnumerable<byte>)Bytes).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Bytes.GetEnumerator();
}
