namespace Bon.Serializer.Test.BonSerialization.Storage;

public sealed class NewLayoutFromStorageTest : StorageTestBase
{
    [Fact]
    public async Task RunAsync()
    {
        var layoutId = BonSerializer.KnownLayouts.Count() + 1;

        // Update the blob contents. This simulates another process writing a new layout to the storage.
        Blob.Bytes = [.. Blob.Bytes, .. GetClassV1Layout(layoutId)];

        Assert.ThrowsAny<Exception>(() => GetSerializationResult(GetClassV1InstanceBytes(layoutId)).DeserializeSlow<ClassV2>());
        await BonSerializer.LoadLatestLayoutsAsync();
        Assert.Equal(ClassV2Instance, GetSerializationResult(GetClassV1InstanceBytes(layoutId)).DeserializeSlow<ClassV2>());
    }
}
