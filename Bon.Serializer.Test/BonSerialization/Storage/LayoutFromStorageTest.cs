namespace Bon.Serializer.Test.BonSerialization.Storage;

public sealed class LayoutFromStorageTest : StorageTestBase
{
    const int LayoutId = 1;

    public LayoutFromStorageTest() : base(new FakeBlob { Bytes = GetClassV1Layout(LayoutId) }) { }

    /// <summary>
    /// Test whether a type that was serialized using a layout that cannot be found via the source generation context but only
    /// via the storage can be deserialized.
    /// </summary>
    [Fact]
    public void Run() => Assert.Equal(
        ClassV2Instance,
        GetSerializationResult(GetClassV1InstanceBytes(LayoutId)).DeserializeSlow<ClassV2>());

    [Fact]
    public void UnknownLayoutId()
    {
        var exception = Assert.ThrowsAny<Exception>(() =>
            GetSerializationResult(GetClassV1InstanceBytes(int.MaxValue)).DeserializeSlow<ClassV2>());

        Assert.Contains("No layout with ID 2147483647 found", exception.Message);
    }
}
