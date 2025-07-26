namespace Bon.Serializer.Test.BonSerialization.Storage;

public class TwoLayoutsFromStorageTest : BonSerializerTestBase
{
    /// <summary>
    /// By using such a large member ID there is practically no chance that the layouts returned by <see cref="GetBlobBytes"/>
    /// already exists somewhere in the source code.
    /// </summary>
    private const int MemberId = 295764629;

    private const int LayoutIdA = 1;
    private const int LayoutIdB = 2;
    private const int Value = 355375878;

    public TwoLayoutsFromStorageTest() : base(new FakeBlob { Bytes = GetBlobBytes() }) { }

    /// <summary>
    /// Test whether a type that was serialized using a layout that cannot be found via the source generation context but only
    /// via the storage can be deserialized.
    /// The type contains another type that cannot be found via the source generation context.
    /// </summary>
    [Fact] public void Run() => Assert.Equal(InstanceD, GetSerializationResult(GetInstanceBBytes()).DeserializeSlow<ClassD>());

    private static byte[] GetBlobBytes()
    {
        var layoutA = new Layout(LayoutIdA, [new SchemaMember(MemberId, Schema.Int)]);
        var layoutB = new Layout(LayoutIdB, [new SchemaMember(MemberId, Schema.Create(SchemaType.NullableRecord, layoutId: LayoutIdA))]);

        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);
        var layoutWriter = new LayoutWriter(writer);
        IntSerializer.WriteNull(writer); // timestamp
        layoutWriter.Write(layoutA);
        IntSerializer.WriteNull(writer); // timestamp
        layoutWriter.Write(layoutB);

        return stream.ToArray();
    }

    private byte[] GetInstanceBBytes() => [.. GetManualSerializer()
        .WriteSchemaType(SchemaType.NullableRecord)
        .WriteCompactInt(LayoutIdB)
        .WriteNotNull()
        .WriteNotNull()
        .WriteFullInt(Value)];

    private static ClassD InstanceD => new(0, new(0, Value));

    // [BonObject] public sealed record class ClassA([property: BonMember(MemberId)] int Y);
    // [BonObject] public sealed record class ClassB([property: BonMember(MemberId)] ClassA Y);
    [BonObject] public sealed record class ClassC([property: BonMember(1)] int X, [property: BonMember(MemberId)] int Y);
    [BonObject] public sealed record class ClassD([property: BonMember(1)] int X, [property: BonMember(MemberId)] ClassC Y);
}
