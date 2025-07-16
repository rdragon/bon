namespace Bon.Serializer.Test.BonSerialization.Storage;

public class UpdateStorageTest : BonSerializerTestBase
{
    [Fact]
    public void WrittenToStorageDuringStartup()
    {
        // We check whether any layouts were saved to the blob during the creation of BonSerializer.
        Assert.True(BonSerializer.FoundNewLayouts);
        Assert.NotEmpty(Blob.Bytes);
    }

    [Fact]
    public async Task AllLayoutsWereWrittenToStorageAsync()
    {
        // During the creation of BonSerializer all layouts should have been written to Blob.
        // We now create a new serializer and check whether no new layouts are written to Blob.
        var expected = Blob.Bytes.Length;
        var secondSerializer = await BonSerializer.CreateAsync(new TestBonSerializerContext(), Blob);
        Assert.Equal(expected, Blob.Bytes.Length);
        Assert.False(secondSerializer.FoundNewLayouts);
    }

    [Fact]
    public async Task CompareSerializersAsync()
    {
        // If we create a new serializer that now uses the updated blob, it should be identical to the original one.
        var secondSerializer = await BonSerializer.CreateAsync(new TestBonSerializerContext(), Blob);
        Assert.Equal(BonSerializer.PrintKnownLayouts(), secondSerializer.PrintKnownLayouts());
        Assert.Equal(BonSerializer.PrintKnownSchemas(), secondSerializer.PrintKnownSchemas());
    }
}
