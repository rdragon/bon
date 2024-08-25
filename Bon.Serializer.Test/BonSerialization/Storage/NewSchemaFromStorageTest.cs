namespace Bon.Serializer.Test.BonSerialization.Storage;

public sealed class NewSchemaFromStorageTest : StorageTestBase
{
    [Fact]
    public void Run()
    {
        // Update the blob contents. This simulates another process writing a new schema to the storage.
        Blob.Bytes = GetSchemaStorageContents();

        Assert.Equal(Instance, GetSerializationResult(GetInstanceBytes()).DeserializeSlow<Class>());
    }
}
