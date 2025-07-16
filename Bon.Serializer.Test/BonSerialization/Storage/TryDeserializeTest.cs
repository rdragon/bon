namespace Bon.Serializer.Test.BonSerialization.Storage;

public class TryDeserializeTest : BonSerializerTestBase
{
    [Fact]
    public void UnknownBlock()
    {
        var stream = new MemoryStream([.. GetManualSerializer().WriteFirstPartOfHeader(0x0d6948da)]);
        Assert.False(BonSerializer.TryDeserialize<int>(stream, out var _));
    }

    [Fact]
    public void ZeroBlock()
    {
        var stream = new MemoryStream([.. GetManualSerializer()
            .WriteFirstPartOfHeader(0)
            .WriteSchemaType(SchemaType.Int)
            .WriteBool(false)
            .WriteFullInt(Int)]);

        Assert.True(BonSerializer.TryDeserialize<int>(stream, out var actual));
        Assert.Equal(Int, actual);
    }
}
