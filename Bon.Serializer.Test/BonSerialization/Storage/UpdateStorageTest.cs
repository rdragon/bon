namespace Bon.Serializer.Test.BonSerialization.Storage;

public class UpdateStorageTest : BonSerializerTestBase
{
    [Fact]
    public void WrittenToStorageDuringStartup()
    {
        Assert.NotEmpty(Blob.Bytes);
    }

    [Fact]
    public async Task AllSchemasWereWrittenToStorage()
    {
        var expected = Blob.Bytes.Length;
        await BonSerializer.CreateAsync(new BonSerializerContext(), Blob);

        Assert.Equal(expected, Blob.Bytes.Length);
    }

    [Fact]
    public async Task CompareSerializers()
    {
        var serializer = await BonSerializer.CreateAsync(new BonSerializerContext(), Blob);
        Assert.Equal(BonSerializer.GetSchemaHash(), serializer.GetSchemaHash());
    }
}
