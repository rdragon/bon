namespace Bon.Serializer.Test.BonSerialization.Storage;

public class StorageTestBase(FakeBlob? blob = null) : BonSerializerTestBase(blob)
{
    /// <summary>
    /// By using such a large member ID there is practically no chance that the schema returned by <see cref="GetClassV1Layout"/>
    /// already exists somewhere in the source code.
    /// </summary>
    private const int MemberId = 295764629;

    private const int Value = 355375878;

    protected static byte[] GetClassV1Layout(int layoutId)
    {
        var layout = new Layout(layoutId, [new SchemaMember(MemberId, Schema.Int)]);
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);
        IntSerializer.WriteNull(writer); // timestamp
        new LayoutWriter(writer).Write(layout);
        return stream.ToArray();
    }

    protected byte[] GetClassV1InstanceBytes(int layoutId) => [.. GetManualSerializer()
        .WriteSchemaType(SchemaType.NullableRecord)
        .WriteCompactInt(layoutId)
        .WriteNotNull()
        .WriteFullInt(Value)];

    protected static ClassV2 ClassV2Instance => new(0, Value);

    // [BonObject] public sealed record class ClassV1([property: BonMember(MemberId)] int Y);
    [BonObject] public sealed record class ClassV2([property: BonMember(1)] int X, [property: BonMember(MemberId)] int Y);
}
