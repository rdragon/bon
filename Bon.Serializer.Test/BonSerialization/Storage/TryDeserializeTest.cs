namespace Bon.Serializer.Test.BonSerialization.Storage;

public class TryDeserializeTest : BonSerializerTestBase
{
    [Fact]
    public void UnknownBlock()
    {
        var stream = new MemoryStream([.. GetSimpleSerializer().WriteFirstPartOfHeader(0x0d6948da)]);
        Assert.False(BonSerializer.TryDeserialize<int>(stream, out var _));
    }

    [Fact]
    public void ZeroBlock()
    {
        var stream = new MemoryStream([.. GetSimpleSerializer()
            .WriteFirstPartOfHeader(0)
            .WriteWholeNumber((int)SchemaType.Int)
            .WriteBool(false)
            .WriteInt(Int)]);

        Assert.True(BonSerializer.TryDeserialize<int>(stream, out var actual));
        Assert.Equal(Int, actual);
    }
}
