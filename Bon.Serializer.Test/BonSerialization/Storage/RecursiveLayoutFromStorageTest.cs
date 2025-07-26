namespace Bon.Serializer.Test.BonSerialization.Storage;

public class RecursiveLayoutFromStorageTest : BonSerializerTestBase
{
    /// <summary>
    /// By using such a large member ID there is practically no chance that the layout returned by <see cref="GetBlobBytes"/>
    /// already exists somewhere in the source code.
    /// </summary>
    private const int MemberId = 295764629;

    private const int LayoutId = 1;
    private const int Value1 = 355375878;
    private const int Value2 = 671172479;

    public RecursiveLayoutFromStorageTest() : base(new FakeBlob { Bytes = GetBlobBytes() }) { }

    /// <summary>
    /// Test whether a recursive type that was serialized using a schema that cannot be found via the source generation context but only
    /// via the storage can be deserialized.
    /// </summary>
    [Fact] public void Run() => Assert.Equal(InstanceV2, GetSerializationResult(GetInstanceV1Bytes()).DeserializeSlow<ClassV2>());

    private static byte[] GetBlobBytes()
    {
        var memberA = new SchemaMember(2, Schema.Create(SchemaType.NullableRecord, layoutId: LayoutId));
        var memberB = new SchemaMember(MemberId, Schema.Int);
        var layout = new Layout(LayoutId, [memberA, memberB]);

        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);
        var layoutWriter = new LayoutWriter(writer);
        IntSerializer.WriteNull(writer); // timestamp
        layoutWriter.Write(layout);

        return stream.ToArray();
    }

    private byte[] GetInstanceV1Bytes() => [.. GetManualSerializer()
        .WriteSchemaType(SchemaType.NullableRecord)
        .WriteCompactInt(LayoutId)
        .WriteNotNull()
        .WriteNotNull()
        .WriteNull()
        .WriteFullInt(Value1)
        .WriteFullInt(Value2)];

    private static ClassV2 InstanceV2 => new(0, new(0, null, Value1), Value2);

    // [BonObject]
    // public sealed record class ClassV1(
    //     [property: BonMember(2)] ClassV1? Y,
    //     [property: BonMember(MemberId)] int Z);

    [BonObject]
    public sealed record class ClassV2(
        [property: BonMember(1)] int X,
        [property: BonMember(2)] ClassV2? Y,
        [property: BonMember(MemberId)] int Z);
}
