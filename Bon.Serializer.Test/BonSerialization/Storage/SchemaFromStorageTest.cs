namespace Bon.Serializer.Test.BonSerialization.Storage;

public sealed class SchemaFromStorageTest : StorageTestBase
{
    public SchemaFromStorageTest() : base(new FakeBlob { Bytes = GetSchemaStorageContents() }) { }

    /// <summary>
    /// Test whether a type that was serialized using a schema that cannot be found via the source generation context but only
    /// via the storage can be deserialized.
    /// </summary>
    [Fact] public void Run() => Assert.Equal(Instance, GetSerializationResult(GetInstanceBytes()).DeserializeSlow<Class>());
}
