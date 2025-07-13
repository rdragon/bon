namespace Bon.Serializer.Test.BonSerialization;

public sealed class SerializationResult(BonSerializer bonSerializer, MemoryStream stream) : IEnumerable<byte>
{
    public byte[] Bytes => stream.ToArray();

    public T DeserializeFast<T>() => Deserialize<T>(false);

    public T DeserializeSlow<T>() => Deserialize<T>(true);

    private T Deserialize<T>(bool? expectNewDeserializer = null)
    {
        stream.Position = 0;
        var countBefore = bonSerializer.DeserializerCount;
        var result = bonSerializer.DeserializeAsync<T>(stream).Result;
        Assert.Equal(stream.Length, stream.Position);

        if (expectNewDeserializer.HasValue)
        {
            var actual = bonSerializer.DeserializerCount != countBefore ? "slow" : "fast";
            Assert.Equal(expectNewDeserializer.Value ? "slow" : "fast", actual);
        }

        return result;
    }

    public string DeserializeToJson() => bonSerializer.BonToJsonAsync(Bytes).Result;

    public IEnumerator<byte> GetEnumerator() => ((IEnumerable<byte>)Bytes).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Bytes.GetEnumerator();
}
