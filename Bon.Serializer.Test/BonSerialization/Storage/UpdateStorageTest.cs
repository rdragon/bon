namespace Bon.Serializer.Test.BonSerialization.Storage;

public class UpdateStorageTest : BonSerializerTestBase
{
    [Fact]
    public void WrittenToStorageDuringStartup()
    {
        // We check whether any layouts were saved to the blob during the creation of BonSerializer.
        Assert.NotEmpty(Blob.Bytes);
    }

    [Fact]
    public async Task AllLayoutsWereWrittenToStorage()
    {
        // During the creation of BonSerializer all layouts should have been written to Blob.
        // We now create a new serializer and check whether no new layouts are written to Blob.
        var expected = Blob.Bytes.Length;
        await BonSerializer.CreateAsync(new TestBonSerializerContext(), Blob);
        Assert.Equal(expected, Blob.Bytes.Length);
    }

    [Fact]
    public async Task CompareSerializers()
    {
        // If we create a new serializer that now uses the updated blob, it should be identical to the original one.
        var serializer = await BonSerializer.CreateAsync(new TestBonSerializerContext(), Blob);
        Assert.Equal(BonSerializer.PrintKnownLayouts(), serializer.PrintKnownLayouts());
        Assert.Equal(BonSerializer.PrintKnownSchemas(), serializer.PrintKnownSchemas());
    }
}
